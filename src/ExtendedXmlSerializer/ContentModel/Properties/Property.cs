// MIT License
// 
// Copyright (c) 2016 Wojciech Nag�rski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.ContentModel.Xml;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	class Property<T> : Identity, IProperty<T>
	{
		readonly ISerializer<T> _serializer;

		public Property(IReader<T> reader, IWriter<T> writer, IIdentity identity)
			: this(
				new Serializer<T>(
					identity is FrameworkIdentity ? new ConfiguredReader<T>(reader, SetContentCommand.Default) : reader,
					writer),
				identity) {}

		public Property(ISerializer<T> serializer, IIdentity identity) : base(identity.Name, identity.Identifier)
		{
			_serializer = serializer;
		}

		public T Get(IContentAdapter parameter) => _serializer.Get(parameter);

		public void Write(IXmlWriter writer, T instance) => _serializer.Write(writer, instance);
	}
}