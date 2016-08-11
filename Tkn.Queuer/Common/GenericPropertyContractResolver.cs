using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Tkn.Queuer.Common {
	public class GenericPropertyContractResolver : CamelCasePropertyNamesContractResolver {
		readonly Type _genericTypeDefinition;

		public GenericPropertyContractResolver(Type genericTypeDefinition) {
			_genericTypeDefinition = genericTypeDefinition;
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
			var baseProperty = base.CreateProperty(member, memberSerialization);

			var declaringType = member.DeclaringType;

			if ((declaringType == null) || !declaringType.GetTypeInfo().IsGenericType ||
				(declaringType.GetGenericTypeDefinition() != _genericTypeDefinition)) {
				return baseProperty;
			}

			var declaringGenericType = declaringType.GetGenericArguments().FirstOrDefault();

			if (IsGenericMember(member) && (declaringGenericType != null))
				baseProperty.PropertyName = ResolvePropertyName(declaringGenericType.Name);

			return baseProperty;
		}

		public bool IsGenericMember(MemberInfo member) {
			var genericMember = _genericTypeDefinition.GetMember(member.Name)[0];

			if (genericMember != null) {
				switch (genericMember.MemberType) {
					case MemberTypes.Field:
						return ((FieldInfo) genericMember).FieldType.IsGenericParameter;
					case MemberTypes.Property:
						var property = (PropertyInfo) genericMember;

						return (property.GetMethod.ReturnParameter != null) && property.GetMethod.ReturnParameter.ParameterType.IsGenericParameter;
				}
			}

			return false;
		}
	}
}
