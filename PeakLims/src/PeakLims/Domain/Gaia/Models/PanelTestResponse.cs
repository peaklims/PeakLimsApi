namespace PeakLims.Domain.Gaia.Models;

using Panels;
using Tests;

public class PanelTestResponse
{
    public List<Test> StandaloneTests { get; set; } = new();
    public List<Panel> Panels { get; set; } = new();
}