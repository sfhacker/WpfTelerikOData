
namespace WpfApplication1
{
    using com.cairone.odataexample;
    using Filters;
    using Microsoft.OData.Client;
    using System;
    using System.Windows;
    using Telerik.Windows.Controls.GridView;

    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var isEnabled = this.RadPagerPaises.IsEnabled;
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
            this.dataServiceDataSource.CancelSubmit();
        }

        private void btnNewCountry_New(object sender, RoutedEventArgs e)
        {
            this.gridView.BeginInsert();
        }

        private void gridView_NewItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
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

            // There is no need to add this validation here
            // How to aviod sending rubbish to the OData Service?
            // Just for testing
            if (cellName.Equals("id"))
            {
                int newValue = Int32.Parse(e.NewValue.ToString());
                if (newValue < 1 || newValue > 99)
                {
                    //e.IsValid = false;
                    //e.ErrorMessage = "The entered value must be between 1 and 99";
                }

                return;
            }

            if (cellName.Equals("nombre"))
            {
                string newValue = e.NewValue.ToString();
                if (string.IsNullOrEmpty(newValue))
                {
                    //e.IsValid = false;
                    //e.ErrorMessage = "'Nombre' cannot be NULL or Empty";
                }

                return;
            }

            if (cellName.Equals("prefijo"))
            {
                string newValue = e.NewValue.ToString();
                if (string.IsNullOrEmpty(newValue))
                {
                    //e.IsValid = false;
                    //e.ErrorMessage = "'Prefijo' cannot be NULL or Empty";
                }

                return;
            }
        }

        private void gridView_RowValidating(object sender, Telerik.Windows.Controls.GridViewRowValidatingEventArgs e)
        {
            // There is no need to add this validation here
            // How to aviod sending rubbish to the OData Service?
            // Just for testing

            Pais country = e.Row.DataContext as Pais;

            /*
            if (string.IsNullOrEmpty(country.nombre) || country.nombre.Length < 3)
            {
                GridViewCellValidationResult validationResult = new GridViewCellValidationResult();
                validationResult.PropertyName = "Nombre";
                validationResult.ErrorMessage = "Nombre is required and must be at least five characters";
                e.ValidationResults.Add(validationResult);
                e.IsValid = false;
            }

            if (country.id < 0)
            {
                GridViewCellValidationResult validationResult = new GridViewCellValidationResult();
                validationResult.PropertyName = "Id";
                validationResult.ErrorMessage = "Id must be positive";
                e.ValidationResults.Add(validationResult);
                e.IsValid = false;
            }*/
        }

        private void gridView_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {
            Pais newPais = null;

            if (e.EditAction == GridViewEditAction.Cancel)
            {
                return;
            }
            if (e.EditOperationType == GridViewEditOperationType.Insert)
            {
                //Add the new entry to the data base.
                if (this.gridView.SelectedItems.Count == 0)
                {
                    return;
                }
                newPais = (Pais)this.gridView.SelectedItems[0];
                if (newPais == null)
                {
                    return;
                }

                //System.Windows.MessageBox.Show(string.Format("New Pais: {0} {1}", newPais.id, newPais.nombre));

                // we could add 'SubmitChanges' method here
                // This is a per row basis update / insert
                return;
            }

            if (e.EditOperationType == GridViewEditOperationType.Edit)
            {
                newPais = (Pais)this.gridView.SelectedItems[0];
                if (newPais == null)
                {
                    return;
                }

                //System.Windows.MessageBox.Show(string.Format("Update Pais: {0} {1}", newPais.id, newPais.nombre));

                // we could add 'SubmitChanges' method here
                // This is a per row basis update / insert
            }

        }

        private void gridView_Loaded(object sender, RoutedEventArgs e)
        {
            this.gridView.KeyboardCommandProvider = new CustomKeyboardCommandProvider(this.gridView);

            //this.gridView.ActionOnLostFocus = ActionOnLostFocus.None; // ?
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

        private void RadPagerPaises_PageIndexChanged(object sender, Telerik.Windows.Controls.PageIndexChangedEventArgs e)
        {
            if (this.gridView.HasItems)
            {
                int skip = (e.NewPageIndex * this.RadPagerPaises.PageSize);
                int take = this.RadPagerPaises.PageSize;

                //this.listBox.ItemsSource = this.data.Skip(e.NewPageIndex * this.radDataPager.PageSize).Take(this.radDataPager.PageSize).ToList();

            }
        }
    }
}
