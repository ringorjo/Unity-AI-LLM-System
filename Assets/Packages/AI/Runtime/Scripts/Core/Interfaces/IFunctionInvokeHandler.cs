namespace Services.AI
{
    public interface IFunctionInvokeHandler
    {
        public string FunctionId { get; }
        public void InvokeFunction(string parametters);
    }

}
