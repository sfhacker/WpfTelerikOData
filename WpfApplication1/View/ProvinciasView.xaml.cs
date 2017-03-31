
namespace WpfApplication1.View
{
    using com.cairone.odataexample;
    using Filters;
    using Microsoft.OData.Client;
    using System;
    using System.Windows;
    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.GridView;
    using Telerik.Windows.Data;

    /// <summary>
    /// Lógica de interacción para ProvinciasView.xaml
    /// </summary>
    public partial class ProvinciasView : Window
    {
        private int paisId = -1;
        public ProvinciasView()
        {
            InitializeComponent();
        }

        public ProvinciasView(int paisId)
            : this()
        {
            this.paisId = paisId;
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

                // The DataPager control is still disabled!
                // only when new item. Delete/Edit work fine.
                if (this.dataServiceDataSource.HasChanges)
                {
                    this.dataServiceDataSource.RejectChanges();
                }
            }
        }

        private void btnCancelAllChanges_Click(object sender, RoutedEventArgs e)
        {
            //this.dataServiceDataSource.CancelSubmit();
            this.dataServiceDataSource.RejectChanges();
        }

        private void btnNewProvincia_New(object sender, RoutedEventArgs e)
        {
            // it does not work any longer if we add filtering
            // there problem was there are two constructors
            this.gridView.BeginInsert();
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
            e.NewObject = new Provincia() { paisId = this.paisId };
        }

        private void gridView_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
        }

        private void gridView_BeginningEdit(object sender, Telerik.Windows.Controls.GridViewBeginningEditRoutedEventArgs e)
        {

        }

        private void gridView_CellValidating(object sender, Telerik.Windows.Controls.GridViewCellValidatingEventArgs e)
        {
        }

        private void gridView_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {
            // if the user pressed ESC key
            // after an INS key
            if (e.EditAction == GridViewEditAction.Cancel)
            {
                if (e.Row.GetType() == typeof(Telerik.Windows.Controls.GridView.GridViewNewRow))
                {
                    (sender as RadGridView).Items.Remove(e.Row.Item);
                }

                return;
            }
            if (e.EditOperationType == GridViewEditOperationType.Insert)
            {
                var newData = e.NewData;
                if (newData != null)
                {
                    if (e.EditAction == Telerik.Windows.Controls.GridView.GridViewEditAction.Commit)
                    {
                    }
                }

                return;
            }
            if (e.EditOperationType == GridViewEditOperationType.Edit)
            {
            }
        }

        private void gridView_Loaded(object sender, RoutedEventArgs e)
        {
            this.gridView.KeyboardCommandProvider = new CustomKeyboardCommandProvider(this.gridView);

            if (this.paisId != -1)
            {
                FilterDescriptor fd_paisId = new FilterDescriptor("paisId", FilterOperator.IsEqualTo, this.paisId, false, typeof(int));
                if (!this.gridView.FilterDescriptors.Contains(fd_paisId))
                {
                    this.gridView.FilterDescriptors.Add(fd_paisId);
                }
            }
        }

        private void OnDataSourceLoadingData(object sender, Telerik.Windows.Controls.DataServices.LoadingDataEventArgs e)
        {
            if (e.Query != null)
            {
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
                System.Windows.MessageBox.Show("Debe seleccionar una provincia.", "Provincias", MessageBoxButton.OK, MessageBoxImage.Stop);

                return;
            }

            var itemsToRemove = new System.Collections.ObjectModel.ObservableCollection<object>();

            foreach (var item in this.gridView.SelectedItems)
            {
                itemsToRemove.Add(item);
            }

            MessageBoxResult rst = System.Windows.MessageBox.Show(string.Format("Esta seguro de querer eliminar {0} item(s)?", itemsToRemove.Count), "Provincias", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (rst == MessageBoxResult.Yes)
            {
                foreach (var item in itemsToRemove)
                {
                    (this.gridView.ItemsSource as Telerik.Windows.Data.DataItemCollection).Remove(item);
                }
            }
            // Can we issue a SubmitChanges() method here instead of clicking 'Save all'
        }

        private void gridView_DataLoaded(object sender, EventArgs e)
        {
            if (this.gridView.CurrentCell != null)
            {
                gridView.CurrentCell.IsSelected = false;
            }

            //var alas = this.dataServiceDataSource.FilterDescriptors.Count;
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

        private void RadPagerProvincias_PageIndexChanged(object sender, Telerik.Windows.Controls.PageIndexChangedEventArgs e)
        {
        }

        private void btnCities_Click(object sender, RoutedEventArgs e)
        {
            if (this.paisId < 1)
            {
                return;
            }
            if (this.gridView.SelectedItems.Count == 0)
            {
                return;
            }
            var newProvincia = (Provincia)this.gridView.SelectedItems[0];
            if (newProvincia == null)
            {
                return;
            }

            var citiesWin = new LocalidadesView(this.paisId, newProvincia.id);

            citiesWin.ShowDialog();
        }

        private void gridView_Filtering(object sender, GridViewFilteringEventArgs e)
        {
        }

        private void gridView_RowValidating(object sender, Telerik.Windows.Controls.GridViewRowValidatingEventArgs e)
        {
            // There is no need to add this validation here
            // How to aviod sending rubbish to the OData Service?
            // Just for testing

            var isValid = e.IsValid;

        }
        private void gridView_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var count = e.ValidationResults.Count;
        }

        private void gridView_DataError(object sender, DataErrorEventArgs e)
        {
            var alas = e.Row;
        }
    }
}
