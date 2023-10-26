using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EmployeesManagement.Common.Models
{
    [DataContract]
    public class Employee
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;
        [DataMember(Name = "email")]
        public string Email { get; set; } = string.Empty;

        [DataMember(Name = "gender")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Gender? Gender { get; set; }

        [DataMember(Name = "status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status? Status { get; set; }
    }

    [DataContract]
    public enum Gender
    {
        [EnumMember(Value = "male")]
        Male,
        [EnumMember(Value = "female")]
        Female,
        [EnumMember(Value = "non-binary")]
        NonBinary,
        NotDefined
    }
    [DataContract]
    public enum Status
    {
        [EnumMember(Value = "active")]
        Active,
        [EnumMember(Value = "inactive")]
        Inactive,
        NotDefined
    }
}
