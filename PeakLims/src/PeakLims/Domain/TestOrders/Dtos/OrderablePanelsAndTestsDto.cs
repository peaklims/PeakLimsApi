namespace PeakLims.Domain.TestOrders.Dtos;

public sealed class OrderablePanelsAndTestsDto
{
    public List<OrderablePanel> Panels { get; set; }
    public List<OrderableTest> Tests { get; set; }

    public sealed class OrderablePanel
    {
        public Guid Id { get; set; }
        public string PanelCode { get; set; }
        public string PanelName { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }
        public string Status { get; set; }
    }
    
    public sealed class OrderableTest
    {
        public Guid Id { get; set; }
        public string TestCode { get; set; }
        public string TestName { get; set; }
        public string Methodology { get; set; }
        public string Platform { get; set; }
        public int Version { get; set; }
        public int TurnAroundTime { get; set; }
        public string Status { get; set; }
    }
}
