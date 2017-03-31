
namespace WpfApplication1.View
{
    using Microsoft.OData.Client;
    using System;
    using System.Windows;
    using Telerik.Windows.Controls.GridView;
    using Telerik.Windows.Data;
    using WpfApplication1.Filters;

    using com.cairone.odataexample;

    /// <summary>
    /// Lógica de interacción para LocalidadesView.xaml
    /// </summary>
    public partial class LocalidadesView : Window
    {
        private const string PAIS_ID_ELEMENT = "paisId";
        private const string PROVINCIA_ID_ELEMENT = "provinciaId";

        private int paisId = -1;
        private int provinciaId = -1;

        public LocalidadesView()
        {
            InitializeComponent();
        }

        public LocalidadesView(int paisId, int provinciaId)
            : this()
        {
            this.paisId = paisId;
            this.provinciaId = provinciaId;
        }

        private void btnSaveAllChanges_Click(object sender, RoutedEventArgs e)
        {
            this.dataServiceDataSource.SubmitChanges();
        }

        private void OnDataSourceSubmittingChanges(object sender, Telerik.Windows.Controls.DataServices.DataServiceSubmittingChangesEventArgs e)
        {
            // I wanted to set this option globally, but it didn't work!
            e.SaveChangesOptions = SaveChangesOptions.ReplaceOnUpdate;
        }

        private void OnDataSourceSubmittedChanges(object sender, Telerik.Windows.Controls.DataServices.DataServiceSubmittedChangesEventArgs e)
        {
            if (e.HasError)
            {
                e.MarkErrorAsHandled();
                this.txtDebug.Text = string.Format("<-- Server returned the following error: {0}\r\n", e.Error);
                if (e.Error.InnerException != null)
                {
                    this.txtDebug.Text += string.Format("<-- Extra error info: {0}\r\n", e.Error.InnerException.Message);
                }
            }
        }

        private void btnCancelAllChanges_Click(object sender, RoutedEventArgs e)
        {
            //this.dataServiceDataSource.CancelSubmit();
            this.dataServiceDataSource.RejectChanges();
        }

        private void btnNewCity_New(object sender, RoutedEventArgs e)
        {
            var rst = this.gridView.BeginInsert();

        }

        /// <summary>
        /// We need this event since there are two constructors
        /// When the user clicks on 'New' button or presses Ins key
        /// nothing happens if we do not add the code below.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView_NewItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            e.NewObject = new Localidad() { paisId = this.paisId, provinciaId = this.provinciaId };
        }

        private void gridView_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
        }

        private void gridView_BeginningEdit(object sender, Telerik.Windows.Controls.GridViewBeginningEditRoutedEventArgs e)
        {

        }

        private void gridView_CellValidating(object sender, Telerik.Windows.Controls.GridViewCellValidatingEventArgs e)
        {
            string cellName = e.Cell.Column.UniqueName;
        }

        private void gridView_RowValidating(object sender, Telerik.Windows.Controls.GridViewRowValidatingEventArgs e)
        {
            // There is no need to add this validation here
            // How to aviod sending rubbish to the OData Service?
            // Just for testing
        }

        private void gridView_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {
            if (e.EditAction == GridViewEditAction.Cancel)
            {
                return;
            }
        }

        private void gridView_Loaded(object sender, RoutedEventArgs e)
        {
            this.gridView.KeyboardCommandProvider = new CustomKeyboardCommandProvider(this.gridView);

            this.gridView.FilterDescriptors.Clear();

            // seems to work
            // but where should i put it
            if (this.paisId != -1 && this.provinciaId != -1)
            {
                //CompositeFilterDescriptor cfd = new CompositeFilterDescriptor();

                FilterDescriptor fd_paisId = new FilterDescriptor(PAIS_ID_ELEMENT, FilterOperator.IsEqualTo, this.paisId);
                FilterDescriptor fd_provinciaId = new FilterDescriptor(PROVINCIA_ID_ELEMENT, FilterOperator.IsEqualTo, this.provinciaId);

                this.gridView.FilterDescriptors.Add(fd_paisId);
                this.gridView.FilterDescriptors.Add(fd_provinciaId);
            }
        }

        private void OnDataSourceLoadingData(object sender, Telerik.Windows.Controls.DataServices.LoadingDataEventArgs e)
        {
            if (e.Query != null)
            {
                // Can we change the query here?
                this.txtDebug.Text = string.Format("--> Requesting {0}\r\n", e.Query.ToString());
            }
        }

        private void OnDataSourceLoadedData(object sender, Telerik.Windows.Controls.DataServices.LoadedDataEventArgs e)
        {
            if (e.HasError)
            {
                e.MarkErrorAsHandled();
                this.txtDebug.Text = string.Format("<-- Server returned the following error: {0}\r\n"
                    , e.Error);
            }
            else if (e.Cancelled)
            {
                this.txtDebug.Text = string.Format("<-- Load operation was cancelled\r\n");
            }
            else
            {
                this.txtDebug.Text += "<-- Server replied in {0} ms\r\n";
            }

            // this value set at whatever value is specified in 'PageSize' xml element
            // if less than the total number of rows
            // i believe this is wrong!
            // Fixed in the service side. Thanks Diego.
            // http://www.telerik.com/forums/raddatapager-not-loading-all-records-when-loading
            // http://www.telerik.com/forums/raddatapager-displays-just-one-page
            var totalCount = e.TotalEntityCount;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.gridView.SelectedItems.Count == 0)
            {
                return;
            }

            var itemsToRemove = new System.Collections.ObjectModel.ObservableCollection<object>();

            foreach (var item in this.gridView.SelectedItems)
            {
                itemsToRemove.Add(item);
            }

            foreach (var item in itemsToRemove)
            {
                (this.gridView.ItemsSource as Telerik.Windows.Data.DataItemCollection).Remove(item);
            }

            // Can we issue a SubmitChanges() method here instead of clicking 'Save all'
        }

        private void gridView_DataLoaded(object sender, EventArgs e)
        {
            if (this.gridView.CurrentCell != null)
            {
                gridView.CurrentCell.IsSelected = false;
            }
        }

        private void gridView_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            // When the grid is first loaded,
            // the first row is always selected
            // if then the user clicks on'Delete' button
            // well, you know what happens
            // is there any other way of doing this?
            if (e.Row.IsSelected)
            {
                e.GridViewDataControl.CurrentItem = null;
            }
        }

        private void RadPagerLocalidades_PageIndexChanged(object sender, Telerik.Windows.Controls.PageIndexChangedEventArgs e)
        {
        }
    }
}
