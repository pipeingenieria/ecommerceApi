using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace eCommerce.EntityModels
{
    public class ProductsEntity : TableEntity
    {
        public ProductsEntity(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        public ProductsEntity() { }

        public string AboutProduct { get; set; }
        public bool AcceptTerms { get; set; }
        public bool AdditionalCertifications { get; set; }
        public string AddressedTo { get; set; }
        public string AreaSubAreaName { get; set; }
        public string ArgumentSale { get; set; }
        public bool ChangeContent { get; set; }
        public string ClassMethodology { get; set; }
        public string Content { get; set; }
        public string CoordinationName { get; set; }
        public string CourseBelongsDiplomaRowKey { get; set; }
        public string CourseBelongsDiplomaName { get; set; }
        public string DiplomaBelongsCourseRowKey { get; set; }
        public string DiplomaBelongsCourseName { get; set; }
        public string Duration { get; set; }
        public string Editor { get; set; }
        public string EmailResponsible { get; set; }
        public string IdAreaSubArea { get; set; }
        public string IdAssignmentsRowKey { get; set; }
        public string IdCoordination { get; set; }
        public string IdResponsible { get; set; }
        public string IdSubCoordination { get; set; }
        public string IdTypeProduct { get; set; }
        public string Image { get; set; }
        public bool IsItHomologated { get; set; }
        public string KeyCode { get; set; }
        public string KnowledgeLevel { get; set; }
        public string MaximumCapacity { get; set; }
        public bool Modular { get; set; }
        public string Name { get; set; }
        public bool NewProduct { get; set; }
        public string OverallObjective { get; set; }
        public string PromotionalVideo { get; set; }
        public string ResponsibleName { get; set; }
        public string State { get; set; }
        public string SubCoordinationName { get; set; }
        public string Teachers { get; set; }
        public string TeachersText { get; set; }
        public string TypeProductName { get; set; }
        
    }
}
