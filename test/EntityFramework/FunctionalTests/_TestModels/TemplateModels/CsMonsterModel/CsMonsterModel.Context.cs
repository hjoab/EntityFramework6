//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FunctionalTests.ProductivityApi.TemplateModels.CsMonsterModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class MonsterModel : DbContext
    {
        public MonsterModel()
            : base("name=MonsterModel")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Another.Place.CustomerMm> Customer { get; set; }
        public DbSet<BarcodeMm> Barcode { get; set; }
        public DbSet<IncorrectScanMm> IncorrectScan { get; set; }
        public DbSet<BarcodeDetailMm> BarcodeDetail { get; set; }
        public DbSet<ComplaintMm> Complaint { get; set; }
        public DbSet<ResolutionMm> Resolution { get; set; }
        public DbSet<Another.Place.LoginMm> Login { get; set; }
        public DbSet<SuspiciousActivityMm> SuspiciousActivity { get; set; }
        public DbSet<SmartCardMm> SmartCard { get; set; }
        public DbSet<RSATokenMm> RSAToken { get; set; }
        public DbSet<PasswordResetMm> PasswordReset { get; set; }
        public DbSet<PageViewMm> PageView { get; set; }
        public DbSet<LastLoginMm> LastLogin { get; set; }
        public DbSet<MessageMm> Message { get; set; }
        public DbSet<OrderMm> Order { get; set; }
        public DbSet<OrderNoteMm> OrderNote { get; set; }
        public DbSet<OrderQualityCheckMm> OrderQualityCheck { get; set; }
        public DbSet<OrderLineMm> OrderLine { get; set; }
        public DbSet<ProductMm> Product { get; set; }
        public DbSet<ProductDetailMm> ProductDetail { get; set; }
        public DbSet<ProductReviewMm> ProductReview { get; set; }
        public DbSet<ProductPhotoMm> ProductPhoto { get; set; }
        public DbSet<ProductWebFeatureMm> ProductWebFeature { get; set; }
        public DbSet<SupplierMm> Supplier { get; set; }
        public DbSet<SupplierLogoMm> SupplierLogo { get; set; }
        public DbSet<SupplierInfoMm> SupplierInfo { get; set; }
        public DbSet<CustomerInfoMm> CustomerInfo { get; set; }
        public DbSet<ComputerMm> Computer { get; set; }
        public DbSet<ComputerDetailMm> ComputerDetail { get; set; }
        public DbSet<DriverMm> Driver { get; set; }
        public DbSet<LicenseMm> License { get; set; }
    
        private ObjectResult<Another.Place.AuditInfoMm> FunctionImport1(ObjectParameter modifiedDate)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Another.Place.AuditInfoMm>("FunctionImport1", modifiedDate);
        }
    
        internal virtual ObjectResult<Another.Place.CustomerMm> FunctionImport2()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Another.Place.CustomerMm>("FunctionImport2");
        }
    
        internal virtual ObjectResult<Another.Place.CustomerMm> FunctionImport2(MergeOption mergeOption)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Another.Place.CustomerMm>("FunctionImport2", mergeOption);
        }
    
        public virtual int ParameterTest(byte[] binary, Nullable<bool> @bool, Nullable<System.DateTime> dateTime, Nullable<decimal> @decimal, Nullable<double> @float, Nullable<System.Guid> guid, ObjectParameter @int, ObjectParameter @string)
        {
            var binaryParameter = binary != null ?
                new ObjectParameter("binary", binary) :
                new ObjectParameter("binary", typeof(byte[]));
    
            var boolParameter = @bool.HasValue ?
                new ObjectParameter("bool", @bool) :
                new ObjectParameter("bool", typeof(bool));
    
            var dateTimeParameter = dateTime.HasValue ?
                new ObjectParameter("dateTime", dateTime) :
                new ObjectParameter("dateTime", typeof(System.DateTime));
    
            var decimalParameter = @decimal.HasValue ?
                new ObjectParameter("decimal", @decimal) :
                new ObjectParameter("decimal", typeof(decimal));
    
            var floatParameter = @float.HasValue ?
                new ObjectParameter("float", @float) :
                new ObjectParameter("float", typeof(double));
    
            var guidParameter = guid.HasValue ?
                new ObjectParameter("guid", guid) :
                new ObjectParameter("guid", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ParameterTest", binaryParameter, boolParameter, dateTimeParameter, decimalParameter, floatParameter, guidParameter, @int, @string);
        }
    
        public virtual ObjectResult<ComputerMm> EntityTypeTest()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ComputerMm>("EntityTypeTest");
        }
    
        public virtual ObjectResult<ComputerMm> EntityTypeTest(MergeOption mergeOption)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ComputerMm>("EntityTypeTest", mergeOption);
        }
    }
}