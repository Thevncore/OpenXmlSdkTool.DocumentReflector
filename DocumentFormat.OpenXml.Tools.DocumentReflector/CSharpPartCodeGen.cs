using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Tools.DocumentReflector.Properties;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal sealed class CSharpPartCodeGen : CSharpCodeGen, IPartContainerMethodBuilder
	{
		private object _partObject;

		private Queue<MethodChunk> _methods = new Queue<MethodChunk>();

		private IDictionary<Uri, string> _dataPartsMap = new Dictionary<Uri, string>();

		private string _methodName = "UnnamedMethod";

		public string MethodName
		{
			get
			{
				return _methodName;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("MethodName could not be empty or null.");
				}
				_methodName = value;
			}
		}

		public MethodAttributes MethodModifier
		{
			get;
			set;
		}

		public int InitialIndent
		{
			get;
			set;
		}

		public CSharpPartCodeGen(OpenXmlPackage package)
		{
			if (package == null)
			{
				throw new ArgumentNullException("package");
			}
			_partObject = package;
			base.GlobalContext.UsedNamespaces.NoAliasNamespaces.Add(NamespaceCollector.GetMainNamespace(package));
		}

		public CSharpPartCodeGen(OpenXmlPart part)
		{
			if (part == null)
			{
				throw new ArgumentNullException("part");
			}
			_partObject = part;
			base.GlobalContext.UsedNamespaces.NoAliasNamespaces.Add(NamespaceCollector.GetMainNamespace(part.OpenXmlPackage));
		}

		public CSharpPartCodeGen(DataPart part)
		{
			_partObject = part;
			base.GlobalContext.UsedNamespaces.NoAliasNamespaces.Add(NamespaceCollector.GetMainNamespace(part.OpenXmlPackage));
		}

		protected override MethodChunk GenerateEntryMethod(int initIndent)
		{
			string methodName = "Create" + _partObject.GetType().Name;
			return BuildMethod(methodName, MethodAttributes.Public, initIndent, base.GlobalContext);
		}

		private MethodChunk BuildMethod(string methodName, MethodAttributes accessFlag, int initIndent, ReflectContext context)
		{
			OpenXmlPackage openXmlPackage = _partObject as OpenXmlPackage;
			bool flag = openXmlPackage != null;
			string text = (flag ? "document" : "part");
			MethodChunk methodChunk = new MethodChunk();
			methodChunk.Head = BuildMethodHead(_partObject, initIndent, accessFlag, methodName, text, context);
			methodChunk.End = BuildMethodEnd(initIndent);
			MethodChunk methodChunk2 = methodChunk;
			_methods.Enqueue(methodChunk2);
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			int indent = initIndent + 4;
			if (flag && openXmlPackage.DataParts.Count() > 0)
			{
				codeChunk.Append(BuildCreateDataPartsStatements(openXmlPackage, text, indent, context, _dataPartsMap));
				codeChunk.AppendLine();
			}
			OpenXmlPartContainer openXmlPartContainer = _partObject as OpenXmlPartContainer;
			if (openXmlPartContainer != null)
			{
				CodeChunk codeChunk2 = ReflectPart(openXmlPartContainer, string.Empty, text, initIndent + 4, new Dictionary<OpenXmlPartContainer, string>(), context, _methods, _dataPartsMap);
				codeChunk.Append(codeChunk2);
			}
			if (!flag)
			{
				AppendGenPartContentStatements(codeChunk, _partObject, text, initIndent + 4, context, _methods);
			}
			methodChunk2.Body = codeChunk;
			return BuildMethodLink(_methods);
		}

		private static CodeChunk BuildMethodHead(object partObj, int indent, MethodAttributes accessFlag, string methodName, string methodParam, ReflectContext context)
		{
			Type type = partObj.GetType();
			context.UsedNamespaces.Collect(type.Namespace);
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			codeChunk.Append(new Indent(indent));
			codeChunk.AppendLine(new Comment("// " + DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.PartCodeEntryComment));
			codeChunk.Append(new Indent(indent));
			codeChunk.Append(new Keyword(CSharpCodeGen.BuildAccessModifier(accessFlag)));
			codeChunk.Append(Keyword.Void, new Separator());
			codeChunk.Append(methodName + "(");
			codeChunk.Append(new TypeMetacode(type.Name));
			codeChunk.AppendLine(" " + methodParam + ")");
			codeChunk.Append(new Indent(indent));
			codeChunk.AppendLine("{");
			return codeChunk;
		}

		private static CodeChunk BuildMethodEnd(int indent)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			codeChunk.Append(new Indent(indent));
			codeChunk.AppendLine("}");
			return codeChunk;
		}

		public static CodeChunk BuildCreateDataPartsStatements(OpenXmlPackage package, string paramName, int indent, ReflectContext context, IDictionary<Uri, string> partUriVarMap)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			bool flag = true;
			foreach (DataPart dataPart in package.DataParts)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					codeChunk.AppendLine();
				}
				IElementCodeBuilder elementCodeBuilder = CSharpCodeGenFactory.CreateDataPartCodeBuilder(dataPart, context, paramName);
				elementCodeBuilder.InitialIndent = indent;
				codeChunk.Append(elementCodeBuilder.Build(out var elementVariableName));
				partUriVarMap.Add(dataPart.Uri, elementVariableName);
			}
			return codeChunk;
		}

		private static MethodChunk BuildMethodLink(Queue<MethodChunk> methods)
		{
			MethodChunk methodChunk = methods.Dequeue();
			MethodChunk methodChunk2 = methodChunk;
			while (methods.Count > 0)
			{
				MethodChunk methodChunk4 = (methodChunk2.Next = methods.Dequeue());
				methodChunk2 = methodChunk4;
			}
			return methodChunk;
		}

		private static CodeChunk ReflectPart(OpenXmlPartContainer partContainer, string relationshipId, string parentVar, int indent, Dictionary<OpenXmlPartContainer, string> processed, ReflectContext context, Queue<MethodChunk> methodQueue, IDictionary<Uri, string> partUriVarMap)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			foreach (IdPartPair part in partContainer.Parts)
			{
				Type type = part.OpenXmlPart.GetType();
				if (!(type == typeof(CoreFilePropertiesPart)))
				{
					CSharpCodeGen.CollectNamespace(context.UsedNamespaces, type);
					if (processed.Keys.Contains(part.OpenXmlPart))
					{
						codeChunk.Append(new Indent(indent), new Metacode(parentVar + ".AddPart(" + processed[part.OpenXmlPart] + ", \"" + part.RelationshipId + "\");"), new LineBreaker());
						codeChunk.AppendLine();
					}
					else
					{
						string text = context.Variables.NewVarName(type);
						AppendAddPartStatements(codeChunk, parentVar, indent, part.OpenXmlPart, part.RelationshipId, text, context);
						AppendGenPartContentStatements(codeChunk, part.OpenXmlPart, text, indent, context, methodQueue);
						processed.Add(part.OpenXmlPart, text);
						codeChunk.Append(ReflectPart(part.OpenXmlPart, part.RelationshipId, text, indent, processed, context, methodQueue, partUriVarMap));
					}
				}
			}
			AppendAddHyperlinksStatements(codeChunk, partContainer, parentVar, indent);
			AppendAddExternalRelationshipsStatements(codeChunk, partContainer, parentVar, indent);
			AppendAddDataPartReferenceRelationshipStatements(codeChunk, partContainer, parentVar, indent, partUriVarMap);
			OpenXmlPackage package;
			if ((package = partContainer as OpenXmlPackage) != null)
			{
				AppendSetPackagePropertiesStatements(codeChunk, package, parentVar, indent, methodQueue);
			}
			return codeChunk;
		}

		private static void AppendAddPartStatements(CodeChunk chunk, string parentVar, int indent, OpenXmlPart part, string relationshipId, string varName, ReflectContext context)
		{
			Type type = part.GetType();
			chunk.Append(new Indent(indent), new TypeMetacode(type.Name), new Metacode(" " + varName + " = " + parentVar));
			if (type == typeof(MainDocumentPart))
			{
				chunk.AppendLine(".AddMainDocumentPart();");
			}
			else if (type == typeof(WorkbookPart))
			{
				chunk.AppendLine(".AddWorkbookPart();");
			}
			else if (type == typeof(PresentationPart))
			{
				chunk.AppendLine(".AddPresentationPart();");
			}
			else if (part is IFixedContentTypePart)
			{
				chunk.Append(".AddNewPart<");
				chunk.Append(new Metacode(context.UsedNamespaces.GetAliasWithDot(type.Namespace)), new TypeMetacode(type.Name));
				chunk.AppendLine(">(\"" + relationshipId + "\");");
			}
			else if (part is ExtendedPart)
			{
				ExtendedPart extendedPart = part as ExtendedPart;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(".AddExtendedPart(\"");
				stringBuilder.Append(extendedPart.RelationshipType);
				stringBuilder.Append("\", \"");
				stringBuilder.Append(extendedPart.ContentType);
				stringBuilder.Append("\", \"");
				string text = extendedPart.Uri.ToString();
				stringBuilder.Append(text.Substring(text.IndexOf(".", StringComparison.Ordinal) + 1));
				stringBuilder.Append("\", \"");
				stringBuilder.Append(relationshipId);
				stringBuilder.Append("\");");
				chunk.AppendLine(stringBuilder.ToString());
			}
			else
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append(".AddNewPart<");
				stringBuilder2.Append(type.Name);
				stringBuilder2.Append(">(\"");
				stringBuilder2.Append(part.ContentType);
				stringBuilder2.Append("\", \"");
				stringBuilder2.Append(relationshipId);
				stringBuilder2.Append("\");");
				chunk.AppendLine(stringBuilder2.ToString());
			}
		}

		private static void AppendGenPartContentStatements(CodeChunk chunk, object part, string varName, int indent, ReflectContext context, Queue<MethodChunk> methodQueue)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "Generate{0}Content", new object[1]
			{
				CapitalizeFirst(varName)
			});
			IPartContentMethodBuilder partContentMethodBuilder = CSharpCodeGenFactory.CreatePartContentMethodBuilder(part);
			partContentMethodBuilder.MethodName = text;
			partContentMethodBuilder.ParamName = varName;
			partContentMethodBuilder.InitialIndent = indent - 4;
			partContentMethodBuilder.Comment = string.Format(CultureInfo.InvariantCulture, DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.PartCodeMethodComment, new object[1]
			{
				varName
			});
			MethodChunk item = partContentMethodBuilder.Build(context);
			methodQueue.Enqueue(item);
			chunk.Append(new Indent(indent));
			chunk.AppendLine(text + "(" + varName + ");");
			chunk.AppendLine();
		}

		private static string CapitalizeFirst(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			if (text.Length != 1)
			{
				return char.ToUpper(text[0], CultureInfo.InvariantCulture) + text.Substring(1);
			}
			return text.ToUpper(CultureInfo.InvariantCulture);
		}

		private static void AppendAddHyperlinksStatements(CodeChunk chunk, OpenXmlPartContainer partContainer, string parentVar, int indent)
		{
			foreach (HyperlinkRelationship hyperlinkRelationship in partContainer.HyperlinkRelationships)
			{
				chunk.Append(new Indent(indent));
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(parentVar);
				stringBuilder.Append(".AddHyperlinkRelationship(new System.Uri(\"");
				stringBuilder.Append(CSharpCodeGen.EscapeCSharpString(hyperlinkRelationship.Uri.OriginalString));
				stringBuilder.Append("\", ");
				stringBuilder.Append(hyperlinkRelationship.Uri.IsAbsoluteUri ? "System.UriKind.Absolute" : "System.UriKind.Relative");
				stringBuilder.Append("), ");
				stringBuilder.Append(hyperlinkRelationship.IsExternal ? "true" : "false");
				stringBuilder.Append(" ,\"");
				stringBuilder.Append(hyperlinkRelationship.Id);
				stringBuilder.Append("\");");
				chunk.AppendLine(stringBuilder.ToString());
			}
		}

		private static void AppendAddExternalRelationshipsStatements(CodeChunk chunk, OpenXmlPartContainer partContainer, string parentVar, int indent)
		{
			foreach (ExternalRelationship externalRelationship in partContainer.ExternalRelationships)
			{
				chunk.Append(new Indent(indent));
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(parentVar);
				stringBuilder.Append(".AddExternalRelationship(\"");
				stringBuilder.Append(CSharpCodeGen.EscapeCSharpString(externalRelationship.RelationshipType));
				stringBuilder.Append("\", new System.Uri(\"");
				stringBuilder.Append(CSharpCodeGen.EscapeCSharpString(externalRelationship.Uri.OriginalString));
				stringBuilder.Append("\", ");
				stringBuilder.Append(externalRelationship.Uri.IsAbsoluteUri ? "System.UriKind.Absolute" : "System.UriKind.Relative");
				stringBuilder.Append("), \"");
				stringBuilder.Append(CSharpCodeGen.EscapeCSharpString(externalRelationship.Id));
				stringBuilder.Append("\");");
				chunk.AppendLine(stringBuilder.ToString());
			}
		}

		private static void AppendAddDataPartReferenceRelationshipStatements(CodeChunk chunk, OpenXmlPartContainer partContainer, string parentVar, int indent, IDictionary<Uri, string> partUriVarMap)
		{
			foreach (DataPartReferenceRelationship dataPartReferenceRelationship in partContainer.DataPartReferenceRelationships)
			{
				string text = string.Empty;
				if (dataPartReferenceRelationship is VideoReferenceRelationship)
				{
					text = "AddVideoReferenceRelationship";
				}
				else if (dataPartReferenceRelationship is AudioReferenceRelationship)
				{
					text = "AddAudioReferenceRelationship";
				}
				else if (dataPartReferenceRelationship is MediaReferenceRelationship)
				{
					text = "AddMediaReferenceRelationship";
				}
				chunk.Append(new Indent(indent));
				if (PartHasMethod(partContainer, text, new Type[2]
				{
					dataPartReferenceRelationship.DataPart.GetType(),
					typeof(string)
				}) && partUriVarMap.Keys.Contains(dataPartReferenceRelationship.Uri))
				{
					chunk.Append(parentVar);
					chunk.Append(".");
					chunk.Append(text);
					chunk.Append("(");
					string text2 = partUriVarMap[dataPartReferenceRelationship.Uri];
					chunk.Append(text2);
					chunk.Append(", ");
					chunk.Append("\"" + dataPartReferenceRelationship.Id + "\"");
					chunk.AppendLine(");");
				}
				else
				{
					chunk.Append(new Comment("//" + DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.PartHasWrongDataPartReferenceRelationshipComment));
					chunk.AppendLine();
				}
			}
		}

		private static bool PartHasMethod(OpenXmlPartContainer part, string methodName, Type[] paramTypes)
		{
			Type type = part.GetType();
			return type.GetMethod(methodName, paramTypes) != null;
		}

		private static void AppendSetPackagePropertiesStatements(CodeChunk chunk, OpenXmlPackage package, string varName, int indent, Queue<MethodChunk> methodQueue)
		{
			string text = "SetPackageProperties";
			MethodChunk item = BuildSetPackagePropertiesMethod(package, text, varName, indent - 4);
			methodQueue.Enqueue(item);
			chunk.Append(new Indent(indent));
			chunk.Append(text);
			chunk.AppendLine("(" + varName + ");");
		}

		private static MethodChunk BuildSetPackagePropertiesMethod(OpenXmlPackage package, string methodName, string paramName, int indent)
		{
			MethodChunk methodChunk = new MethodChunk();
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			codeChunk.Append(new Indent(indent));
			codeChunk.Append(Keyword.Private, new Separator());
			codeChunk.Append(Keyword.Void, new Separator());
			codeChunk.Append(methodName + "(");
			codeChunk.Append(new TypeMetacode(typeof(OpenXmlPackage).Name), new Separator());
			codeChunk.AppendLine(paramName + ")");
			codeChunk.Append(new Indent(indent));
			codeChunk.AppendLine("{");
			CodeChunk codeChunk2 = CodeChunk.CreateDefault();
			codeChunk2.Append(new Indent(indent));
			codeChunk2.AppendLine("}");
			indent += 4;
			CodeChunk codeChunk3 = CodeChunk.CreateDefault();
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
			PropertyInfo[] properties = package.PackageProperties.GetType().GetProperties(bindingFlags);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				object obj = package.PackageProperties.GetType().InvokeMember(propertyInfo.Name, bindingFlags | BindingFlags.GetProperty, Type.DefaultBinder, package.PackageProperties, null, CultureInfo.InvariantCulture);
				if (obj != null)
				{
					codeChunk3.Append(new Indent(indent));
					string str = paramName + ".PackageProperties." + propertyInfo.Name;
					if (propertyInfo.PropertyType == typeof(string))
					{
						codeChunk3.AppendLine(str + " = \"" + obj.ToString() + "\";");
					}
					else if (propertyInfo.PropertyType == typeof(DateTime?))
					{
						string str2 = "System.Xml.XmlConvert.ToDateTime(\"" + CSharpCodeGen.EscapeCSharpString(XmlConvert.ToString((DateTime)obj, XmlDateTimeSerializationMode.Utc)) + "\", System.Xml.XmlDateTimeSerializationMode.RoundtripKind);";
						codeChunk3.AppendLine(str + " = " + str2);
					}
				}
			}
			methodChunk.Head = codeChunk;
			methodChunk.Body = codeChunk3;
			methodChunk.End = codeChunk2;
			return methodChunk;
		}

		public MethodChunk Build(ReflectContext context)
		{
			return BuildMethod(MethodName, MethodModifier, InitialIndent, context);
		}
	}
}
