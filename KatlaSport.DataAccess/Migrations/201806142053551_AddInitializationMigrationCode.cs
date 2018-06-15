namespace KatlaSport.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// Migration for set ManufacturerCode property as IsRequared
    /// </summary>
    public partial class AddInitializationMigrationCode : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.catalogue_products", "product_manufacturer_code", c => c.String(nullable: false, maxLength: 10));
        }

        public override void Down()
        {
            AlterColumn("dbo.catalogue_products", "product_manufacturer_code", c => c.String(maxLength: 10));
        }
    }
}
