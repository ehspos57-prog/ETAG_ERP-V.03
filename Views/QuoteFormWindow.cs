namespace ETAG_ERP.Views
{
    internal class QuoteFormWindow
    {
        private Invoice selectedQuote;

        public QuoteFormWindow()
        {
        }

        public QuoteFormWindow(Invoice selectedQuote)
        {
            this.selectedQuote = selectedQuote;
        }

        internal bool ShowDialog()
        {
            throw new NotImplementedException();
        }
    }
}