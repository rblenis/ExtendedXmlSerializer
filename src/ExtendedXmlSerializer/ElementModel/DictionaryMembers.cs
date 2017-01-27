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

using System.Collections;
using System.Reflection;
using ExtendedXmlSerialization.ElementModel.Members;

namespace ExtendedXmlSerialization.ElementModel
{
	public class DictionaryMembers : Members.Members
	{
		public DictionaryMembers(IElement key, IElement value) : this(DictionaryEntryElement.Item.Classification, key, value) {}

		DictionaryMembers(TypeInfo classification, IElement key, IElement value)
			: this(
				classification.GetProperty(nameof(DictionaryEntry.Key)), classification.GetProperty(nameof(DictionaryEntry.Value)),
				GetterFactory.Default, SetterFactory.Default, key, value) {}

		DictionaryMembers(MemberInfo key, MemberInfo value, IGetterFactory getter, ISetterFactory setter,
		                  IElement keyElement, IElement valueElement)
			: base(
				new Member(key, setter.Get(key), getter.Get(key), keyElement),
				new Member(value, setter.Get(value), getter.Get(value), valueElement)
			) {}
	}
}