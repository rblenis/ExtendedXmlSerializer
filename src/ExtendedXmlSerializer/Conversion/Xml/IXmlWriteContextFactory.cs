using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ElementModel;
using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.ElementModel.Names;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public interface IXmlWriteContextFactory : ICommand<string>, IServiceProvider
	{
		IDisposable Start(IWriteContext context);

		IWriteContext Create(IContainer container, TypeInfo instanceType);
	}

	public interface IEmitter : ICommand<IWriteContext> {}

	public abstract class EmitterBase : IEmitter
	{
		public void Execute(IWriteContext parameter)
		{
			var name = parameter.Container as IName ?? (parameter.Element as INamedElement)?.Name;
			Emit(parameter, name);
		}

		protected abstract void Emit(IWriteContext parameter, IName name);
	}

	class Emitter : EmitterBase
	{
		readonly INames _names;
		readonly XmlWriter _writer;

		public Emitter(INames names, XmlWriter writer)
		{
			_names = names;
			_writer = writer;
		}

		protected override void Emit(IWriteContext parameter, IName name)
		{
			var data = _names.Get(name);
			_writer.WriteStartElement(data.LocalName, data.NamespaceName);
		}
	}

	class QualifiedNameTypeFormatter : CacheBase<TypeInfo, string>, ITypeFormatter
	{
		readonly ITypeNames _names;

		public QualifiedNameTypeFormatter(ITypeNames names)
		{
			_names = names;
		}

		protected override string Create(TypeInfo parameter)
		{
			var name = _names.Get(parameter);
			var result = XmlQualifiedName.ToString(name.LocalName, name.NamespaceName);
			return result;
		}
	}

	public interface ITypeNames : IParameterizedSource<TypeInfo, XName> {}

	class TypeNames : CacheBase<TypeInfo, XName>, ITypeNames
	{
		readonly ElementModel.Names.INames _names;
		readonly INames _native;

		public TypeNames(ElementModel.Names.INames names, INames native)
		{
			_names = names;
			_native = native;
		}

		protected override XName Create(TypeInfo parameter) => _native.Get(_names.Get(parameter));
	}

	public interface IGenericName : IName
	{
		ImmutableArray<IName> Arguments { get; }
	}

	class MemberEmitter : EmitterBase
	{
		readonly ITypeNames _names;
		readonly XName _type;
		readonly XmlWriter _writer;

		public MemberEmitter(ITypeNames names, INames converter, IName type, XmlWriter writer)
			: this(names, converter.Get(type), writer) {}

		public MemberEmitter(ITypeNames names, XName type, XmlWriter writer)
		{
			_names = names;
			_type = type;
			_writer = writer;
		}

		protected override void Emit(IWriteContext parameter, IName name)
		{
			_writer.WriteStartElement(name.DisplayName);

			var classification = parameter.Element.Classification;
			if (!parameter.Container.Exact(classification))
			{
				var native = _names.Get(classification);
				_writer.WriteStartAttribute(_type.LocalName, _type.NamespaceName);
				_writer.WriteQualifiedName(native.LocalName, native.NamespaceName);
				_writer.WriteEndAttribute();
			}
		}
	}


	class XmlWriteContextFactory : IXmlWriteContextFactory
	{
		readonly IElements _elements;
		readonly INamespaces _namespaces;
		readonly XmlWriter _writer;
		readonly IDisposable _finish;
		readonly ImmutableArray<object> _services;

		public XmlWriteContextFactory(IElements elements, INamespaces namespaces, XmlWriter writer)
			: this(elements, namespaces, writer, writer) {}

		public XmlWriteContextFactory(IElements elements, INamespaces namespaces, XmlWriter writer, params object[] services)
			: this(elements, namespaces, writer, new DelegatedDisposable(writer.WriteEndElement), services.ToImmutableArray()) {}

		public XmlWriteContextFactory(IElements elements, INamespaces namespaces, XmlWriter writer, IDisposable finish,
		                              ImmutableArray<object> services)
		{
			_elements = elements;
			_namespaces = namespaces;
			_writer = writer;
			_finish = finish;
			_services = services;
		}

		public IWriteContext Create(IContainer container, TypeInfo instanceType)
		{
			return new XmlWriteContext(this, container, _elements.Load(container, instanceType));
		}

		public object GetService(Type serviceType)
		{
			var info = serviceType.GetTypeInfo();
			var length = _services.Length;
			for (var i = 0; i < length; i++)
			{
				var service = _services[i];
				if (info.IsInstanceOfType(service))
				{
					return service;
				}
			}
			return null;
		}

		public IDisposable Start(IWriteContext context)
		{
			var name = context.Container as IName ?? (context.Element as INamedElement)?.Name;
			if (name != null)
			{
				if (context.Container is IMember)
				{
					_writer.WriteStartElement(name.DisplayName);

					/*var type = instance.GetType().GetTypeInfo();
					if (!context.Container.Exact(type))
					{
						context.Write(TypeProperty.Default, type);
					}*/
				}
				else
				{
					_writer.WriteStartElement(name.DisplayName, _namespaces.Get(name.Classification).Namespace.NamespaceName);
				}

				return _finish;
			}
			throw new SerializationException(
				$"Display name not found for element '{context.Element}' within a container of '{context.Container}.'");
		}

		public void Execute(string parameter) => _writer.WriteString(parameter);
	}
}