
namespace WpfApplication1
{
    using com.cairone.odataexample;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class ODataServiceContext : ODataExample
    {
        public ODataServiceContext()
            : base(new System.Uri("http://localhost:8080/odata/appexample.svc/"))
        {
            // Always PUT method or operation
            /*
            this.SaveChangesDefaultOptions = Microsoft.OData.Client.SaveChangesOptions.ReplaceOnUpdate;

            // Uncomment the code below if the OData Service requires authentication
            this.SendingRequest2 += (sender, eventArgs) =>
            {
                eventArgs.RequestMessage.SetHeader(headerName: "Authorization",
                    headerValue: "Basic YWRta.....");
            };*/
        }

        private IList<Pais> GetAllPaises()
        {
            return new ObservableCollection<Pais>(this.Paises);
        }
    }
}