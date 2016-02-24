#High-level design overview

**NuClear River** platform consists of two components: [Querying](../terms.md) and [Replication](../terms.md). Technically, they are independent from each other. But [metadata descriptions][metadata-descriptions] used for both to describe a specific bounded context should be coordinated.

Let's take a look at **Querying** first. It's a very simple application that exposed API throught HTTP in terms of [OData](http://www.odata.org/) protocol. This application built on ASP.NET Web API and uses [Microsoft.AspNet.OData](https://www.nuget.org/packages/Microsoft.AspNet.OData/) and [EntityFramework](https://www.nuget.org/packages/EntityFramework) libraries.

So, OData protocol usage gives us two main profits:

* OData metadata, a machine-readable description of the data model of the APIs
* An opportunity to client to construct an arbitrary query based on OData metadata

OData service uses an abstract data model called [Entity Data Model (EDM)](https://msdn.microsoft.com/library/ee382825.aspx) to describe the exposed data in the service. To construct EDM model **Querying** component uses [metadata descriptions][metadata-descriptions]. Moreover, concept of bounded contexts is fully supported, so you can have as many EDM models as you need. From OData side, bounded contexts are implemented with [Entity containers](http://docs.oasis-open.org/odata/odata/v4.0/errata02/os/complete/part3-csdl/odata-v4.0-errata02-os-part3-csdl-complete.html#_Toc406398024).

It's important that [metadata descriptions][metadata-descriptions] describe not only EDM model, but the API of **Querying** too. Hense, you just need to create metadata for your bounded context and **Querying** component in will start to use it to execute queries for Read Model.

Here is the example. Say, bounded context looks like this:

![image](../diagrams/conceptual-model-example.png)

Here we have one aggregate - `Project`. It has two entities except aggregate root - `Category` and `Firm`, and two value objects - `FirmBalance` and `FirmCategory` that related to `Firm` entity.

So, let's describe it with [metadata descriptions][metadata-descriptions]:

```csharp
StructuralModelElementBuilder ConceptualModel =
StructuralModelElement.Config
    .Elements(
    	EntityElement.Config.Name(EntityName.Project).EntitySetName("Projects")
            .HasKey("Id")
            .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
            .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
            .Relation(EntityRelationElement.Config.Name("Categories").DirectTo(EntityElement.Config.Name(EntityName.Category)).AsMany().AsContainment())
            .Relation(EntityRelationElement.Config.Name("Firms").DirectTo(EntityElement.Config.Name(EntityName.Firm)).AsMany().AsContainment()),

        EntityElement.Config
        	.Name(EntityName.Category).EntitySetName("Categories")
            .HasKey("CategoryId")
            .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
            .Property(EntityPropertyElement.Config.Name("ProjectId").OfType(ElementaryTypeKind.Int64))
            .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
            .Property(EntityPropertyElement.Config.Name("Level").OfType(ElementaryTypeKind.Int32)),

         EntityElement.Config
         	.Name(EntityName.Firm).EntitySetName("Firms")
	        .HasKey("Id")
	        .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
	        .Property(EntityPropertyElement.Config.Name("ProjectId").OfType(ElementaryTypeKind.Int64))
	        .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
	        .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(ElementaryTypeKind.DateTimeOffset))
	        .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(ElementaryTypeKind.Boolean))
	        .Relation(EntityRelationElement.Config.Name("Balances")
	            .DirectTo(
	                EntityElement.Config
	                	.Name(EntityName.FirmBalance)
	                    .HasKey("AccountId")
	                    .Property(EntityPropertyElement.Config.Name("AccountId").OfType(ElementaryTypeKind.Int64))
	                    .Property(EntityPropertyElement.Config.Name("ProjectId").OfType(ElementaryTypeKind.Int64))
	                    .Property(EntityPropertyElement.Config.Name("Balance").OfType(ElementaryTypeKind.Decimal))
	            ).AsMany())
 	        .Relation(EntityRelationElement.Config.Name("Categories")
	            .DirectTo(
	                EntityElement.Config
	                	.Name(EntityName.FirmCategory)
	                    .HasKey("CategoryId")
	                    .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
	                    .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
	                    .Property(EntityPropertyElement.Config.Name("AdvertisersShare").OfType(ElementaryTypeKind.Double))
	            ).AsMany().AsContainment()));
```

Having this description of bounded context, **Querying** produces the following OData metadata:

```xml
<?xml version="1.0" encoding="utf-8"?>
<edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
  <edmx:DataServices>
    <Schema Namespace="Querying.CustomerIntelligence" xmlns="http://docs.oasis-open.org/odata/ns/edm">
    	<EntityType Name="Project">
         <Key>
          <PropertyRef Name="Id" />
         </Key>
        <Property Name="Id" Type="Edm.Int64" Nullable="false" />
        <Property Name="Name" Type="Edm.String" Nullable="false" />
        <NavigationProperty Name="Categories" Type="Collection(Querying.CustomerIntelligence.Category)" ContainsTarget="true" />
        <NavigationProperty Name="Firms" Type="Collection(Querying.CustomerIntelligence.Firm)" ContainsTarget="true" />
      </EntityType>
      <EntityType Name="Category">
        <Key>
          <PropertyRef Name="CategoryId" />
        </Key>
        <Property Name="CategoryId" Type="Edm.Int64" Nullable="false" />
        <Property Name="ProjectId" Type="Edm.Int64" Nullable="false" />
        <Property Name="Name" Type="Edm.String" Nullable="false" />
        <Property Name="Level" Type="Edm.Int32" Nullable="false" />
      </EntityType>
      <EntityType Name="FirmBalance">
        <Key>
          <PropertyRef Name="AccountId" />
        </Key>
        <Property Name="AccountId" Type="Edm.Int64" Nullable="false" />
        <Property Name="ProjectId" Type="Edm.Int64" Nullable="false" />
        <Property Name="Balance" Type="Edm.Decimal" Nullable="false" />
      </EntityType>
      <EntityType Name="FirmCategory">
        <Key>
          <PropertyRef Name="CategoryId" />
        </Key>
        <Property Name="CategoryId" Type="Edm.Int64" Nullable="false" />
        <Property Name="Name" Type="Edm.String" Nullable="false" />
        <Property Name="AdvertisersShare" Type="Edm.Double" Nullable="false" />
      </EntityType>
      <EntityType Name="Firm">
        <Key>
          <PropertyRef Name="Id" />
        </Key>
        <Property Name="Id" Type="Edm.Int64" Nullable="false" />
        <Property Name="ProjectId" Type="Edm.Int64" Nullable="false" />
        <Property Name="Name" Type="Edm.String" Nullable="false" />
        <Property Name="CreatedOn" Type="Edm.DateTimeOffset" Nullable="false" />
        <Property Name="HasPhone" Type="Edm.Boolean" Nullable="false" />
        <NavigationProperty Name="Balances" Type="Collection(Querying.CustomerIntelligence.FirmBalance)" />
        <NavigationProperty Name="Categories" Type="Collection(Querying.CustomerIntelligence.FirmCategory)" ContainsTarget="true" />
      </EntityType>
      <EntityContainer Name="DefaultContainer">
        <EntitySet Name="Projects" EntityType="Querying.CustomerIntelligence.Project" />
        <EntitySet Name="Categories" EntityType="Querying.CustomerIntelligence.Category" />
        <EntitySet Name="Firms" EntityType="Querying.CustomerIntelligence.Firm" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
```

Next, a client application can use this OData metadata to build bounded context's model on the client side (for example, with [ODataJS](http://olingo.apache.org/doc/javascript/index.html) library, or [many others](http://www.odata.org/libraries/)) to compose REST queries using OData syntax. 

So, these description is the main part of **Querying's** configuration to customize its behaivour for your bounded context. There is some more detais about DI and mappings and configuration of data storage, you can find them in [Querying design](querying-design.md) article.

Now, let's take a look at **Replication** component. It's a bit more complicated. 

First of all, it's important to know that it's control flow consists at least of two parts/stages - _primary_ and _final_. These terms also come from the [NuClear.Operations* libraries](../dependencies/nuclear-operations-libraries.md) used for extenal events processing.

There are main things we need to understand at this point:
* The _primary_ stage used for extenal events receiving and processing. The main goal here is to sync [_facts storage_](../terms.md) with storage of the source system and generate commands to be processed on the next stage
* The _final_ stage is responsible for receiving and processing of commands comes from the _primary_ stage to construct [aggregates](../terms.md) using _facts storage_

In fact, these stages can be perceived as [ETL](https://en.wikipedia.org/wiki/Extract,_transform,_load)-pipeline. We extract data at _primary_ stage and then transform and load data at _final_ stage.

So, as far as _primary_ stage needed for data extraction, we can plug many source systems to **NuClear River** to compose data of bounded context from many sources.

Similar technique used at _final_ stage. We can design aggregates in a way they consist of _mandatory_ and _optional_ objects. Mandatory objects is the set of entities and value object that define the meaning of the aggregate. Optional objects are value objects that supplement aggregate. So, we can have many _final_ stages for the same aggregate - one to deal with mandatory objects and others (_main flow_) - to optional objects (_additional flows_). 

To get more details on **Replication** see [Replication design](replication-design.md) article.

[metadata-descriptions]: ../terms.md