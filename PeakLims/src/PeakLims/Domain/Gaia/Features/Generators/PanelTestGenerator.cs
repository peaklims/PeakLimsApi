namespace PeakLims.Domain.Gaia.Features.Generators;

using Databases;
using Models;
using Panels;
using Panels.Models;
using Tests;
using Tests.Models;

public interface IPanelTestGenerator
{
    Task<PanelTestResponse> Generate(Guid organizationId);
}

public class PanelTestGenerator(PeakLimsDbContext dbContext) : IPanelTestGenerator
{
    public async Task<PanelTestResponse> Generate(Guid organizationId)
    {
        var fullTestList = new PanelTestResponse();
        var panelResponseOpticalGenomeMapping = await AddOpticalGenomeMapping(organizationId);
        var panelResponseWholeGenomeSequencing = await AddWholeGenomeSequencing(organizationId);
        var panelResponseTranscriptomeSequencing = await AddTranscriptomeSequencing(organizationId);
        var panelResponseCombinedSequencing = await AddCombinedSequencing(organizationId);
        var panelResponseCombinedOgmWgsSequencing = await AddCombinedOgmWgsSequencing(organizationId);
        var panelResponseCombinedOgmExternalDataAnalysis = await AddCombinedOgmExternalDataAnalysis(organizationId);
        var panelResponseBasicExomeSequencing = await AddBasicExomeSequencing(organizationId);
        var panelResponseExpandedExomeSequencing = await AddExpandedExomeSequencing(organizationId);
        fullTestList.Panels.AddRange([
            ..panelResponseOpticalGenomeMapping.Panels,
            ..panelResponseWholeGenomeSequencing.Panels,
            ..panelResponseTranscriptomeSequencing.Panels,
            ..panelResponseCombinedSequencing.Panels,
            ..panelResponseCombinedOgmWgsSequencing.Panels,
            ..panelResponseCombinedOgmExternalDataAnalysis.Panels,
            ..panelResponseBasicExomeSequencing.Panels,
            ..panelResponseExpandedExomeSequencing.Panels
        ]);
        
        var kfvTest = Test.Create(new TestForCreation()
        {
            TestCode = "KFV001",
            TestName = "Known Familial Variant (Single Target)",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();
        var pgxAddOnTest = Test.Create(new TestForCreation()
        {
            TestCode = "PGX001",
            TestName = "Pharmacogenomics Add-On",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();
        var hlaAddOnTest = Test.Create(new TestForCreation()
        {
            TestCode = "HLA001",
            TestName = "HLA Typing Add-On",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();
        var pkdAddOnTest = Test.Create(new TestForCreation()
        {
            TestCode = "PKD001",
            TestName = "PKD1 Long Read Sequencing Add-On",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();
        var wgaAddOnTest = Test.Create(new TestForCreation()
        {
            TestCode = "WGA001",
            TestName = "Whole Genome Amplification Add-On",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();

        await dbContext.Tests.AddRangeAsync(kfvTest, pgxAddOnTest, hlaAddOnTest, pkdAddOnTest, wgaAddOnTest);
        fullTestList.StandaloneTests.AddRange([kfvTest, pgxAddOnTest, hlaAddOnTest, pkdAddOnTest, wgaAddOnTest]);
        return fullTestList;
    }

    private async Task<PanelTestResponse> AddWholeGenomeSequencing(Guid organizationId)
    {
        var probandWgsTest = Test.Create(new TestForCreation()
        {
            TestCode = "TWGS001",
            TestName = "Whole Genome Sequencing (Proband)",
            OrganizationId = organizationId,
            Methodology = "Whole Genome Sequencing"
        }).Activate();

        var panelWgsProband = Panel.Create(new PanelForCreation()
        {
            PanelCode = "WGS001",
            PanelName = "Whole Genome Sequencing Proband",
            Type = "WGS",
            OrganizationId = organizationId
        });
        panelWgsProband.AddTest(probandWgsTest).Activate();

        var additionalFamilyMemberWgsTestOne = Test.Create(new TestForCreation()
        {
            TestCode = "TWGS002",
            TestName = "Whole Genome Sequencing (Additional Family Member 1)",
            OrganizationId = organizationId,
            Methodology = "Whole Genome Sequencing"
        }).Activate();

        var panelWgsDuo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "WGS001",
            PanelName = "Whole Genome Sequencing Duo",
            Type = "WGS",
            OrganizationId = organizationId
        });
        panelWgsDuo.AddTest(probandWgsTest)
                   .AddTest(additionalFamilyMemberWgsTestOne)
                   .Activate();

        var additionalFamilyMemberWgsTestTwo = Test.Create(new TestForCreation()
        {
            TestCode = "TWGS003",
            TestName = "Whole Genome Sequencing (Additional Family Member 2)",
            OrganizationId = organizationId,
            Methodology = "Whole Genome Sequencing"
        }).Activate();

        var panelWgsTrio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "WGS001",
            PanelName = "Whole Genome Sequencing Trio",
            Type = "WGS",
            OrganizationId = organizationId
        });
        panelWgsTrio.AddTest(probandWgsTest)
                   .AddTest(additionalFamilyMemberWgsTestOne)
                   .AddTest(additionalFamilyMemberWgsTestTwo)
                   .Activate();
        
        var additionalFamilyMemberWgsTestThree = Test.Create(new TestForCreation()
        {
            TestCode = "TWGS004",
            TestName = "Whole Genome Sequencing (Additional Family Member 3)",
            OrganizationId = organizationId,
            Methodology = "Whole Genome Sequencing"
        }).Activate();
        
        var panelWgsQuartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "WGS001",
            PanelName = "Whole Genome Sequencing Quartet",
            Type = "WGS",
            OrganizationId = organizationId
        });
        panelWgsQuartet.AddTest(probandWgsTest)
                     .AddTest(additionalFamilyMemberWgsTestOne)
                     .AddTest(additionalFamilyMemberWgsTestTwo)
                     .AddTest(additionalFamilyMemberWgsTestThree)
                     .Activate();
        
        await dbContext.Panels.AddRangeAsync(panelWgsProband, panelWgsDuo, panelWgsTrio, panelWgsQuartet);

        return new PanelTestResponse()
        {
            Panels = [panelWgsProband, panelWgsDuo, panelWgsTrio, panelWgsQuartet],
        };
    }
    
    private async Task<PanelTestResponse> AddOpticalGenomeMapping(Guid organizationId)
    {
        var probandOgmTest = Test.Create(new TestForCreation()
        {
            TestCode = "TOGM001",
            TestName = "Optical Genome Mapping (Proband)",
            OrganizationId = organizationId,
            Methodology = "Optical Genome Mapping"
        }).Activate();
        var panelOgmProband = Panel.Create(new PanelForCreation()
        {
            PanelCode = "OGM001",
            PanelName = "Optical Genome Mapping Proband",
            Type = "OGM",
            OrganizationId = organizationId
        });
        panelOgmProband.AddTest(probandOgmTest).Activate();
        
        var additionalFamilyMemberOgmTestOne = Test.Create(new TestForCreation()
        {
            TestCode = "TOGM002",
            TestName = "Optical Genome Mapping (Additional Family Member 1)",
            OrganizationId = organizationId,
            Methodology = "Optical Genome Mapping"
        }).Activate();
        var panelOgmDuo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "OGM001",
            PanelName = "Optical Genome Mapping Duo",
            Type = "OGM",
            OrganizationId = organizationId
        });
        panelOgmDuo.AddTest(probandOgmTest)
            .AddTest(additionalFamilyMemberOgmTestOne)
            .Activate();
        var additionalFamilyMemberOgmTestTwo = Test.Create(new TestForCreation()
        {
            TestCode = "TOGM003",
            TestName = "Optical Genome Mapping (Additional Family Member 2)",
            OrganizationId = organizationId,
            Methodology = "Optical Genome Mapping"
        }).Activate();
        var panelOgmTrio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "OGM001",
            PanelName = "Optical Genome Mapping Trio",
            Type = "OGM",
            OrganizationId = organizationId
        });
        panelOgmTrio.AddTest(probandOgmTest)
            .AddTest(additionalFamilyMemberOgmTestOne)
            .AddTest(additionalFamilyMemberOgmTestTwo)
            .Activate();
        
        var additionalFamilyMemberOgmTestThree = Test.Create(new TestForCreation()
        {
            TestCode = "TOGM004",
            TestName = "Optical Genome Mapping (Additional Family Member 3)",
            OrganizationId = organizationId,
            Methodology = "Optical Genome Mapping"
        }).Activate();
        var panelOgmQuartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "OGM001",
            PanelName = "Optical Genome Mapping Quartet",
            Type = "OGM",
            OrganizationId = organizationId
        });
        panelOgmQuartet.AddTest(probandOgmTest)
            .AddTest(additionalFamilyMemberOgmTestOne)
            .AddTest(additionalFamilyMemberOgmTestTwo)
            .AddTest(additionalFamilyMemberOgmTestThree)
            .Activate();
        
        await dbContext.Panels.AddRangeAsync(panelOgmProband, panelOgmDuo, panelOgmTrio, panelOgmQuartet);

        return new PanelTestResponse()
        {
            Panels = [panelOgmProband, panelOgmDuo, panelOgmTrio, panelOgmQuartet],
        };
    }
    
    private async Task<PanelTestResponse> AddTranscriptomeSequencing(Guid organizationId)
    {
        var probandTranTest = Test.Create(new TestForCreation()
        {
            TestCode = "TTRAN001",
            TestName = "Transcriptome Sequencing (Proband)",
            OrganizationId = organizationId,
            Methodology = "Transcriptome Sequencing"
        }).Activate();

        var panelTranProband = Panel.Create(new PanelForCreation()
        {
            PanelCode = "TRAN001",
            PanelName = "Transcriptome Sequencing Proband",
            Type = "TRAN",
            OrganizationId = organizationId
        });
        panelTranProband.AddTest(probandTranTest).Activate();

        var additionalFamilyMemberTranTestOne = Test.Create(new TestForCreation()
        {
            TestCode = "TTRAN002",
            TestName = "Transcriptome Sequencing (Additional Family Member 1)",
            OrganizationId = organizationId,
            Methodology = "Transcriptome Sequencing"
        }).Activate();

        var panelTranDuo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "TRAN001",
            PanelName = "Transcriptome Sequencing Duo",
            Type = "TRAN",
            OrganizationId = organizationId
        });
        panelTranDuo.AddTest(probandTranTest)
                   .AddTest(additionalFamilyMemberTranTestOne)
                   .Activate();

        var additionalFamilyMemberTranTestTwo = Test.Create(new TestForCreation()
        {
            TestCode = "TTRAN003",
            TestName = "Transcriptome Sequencing (Additional Family Member 2)",
            OrganizationId = organizationId,
            Methodology = "Transcriptome Sequencing"
        }).Activate();

        var panelTranTrio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "TRAN001",
            PanelName = "Transcriptome Sequencing Trio",
            Type = "TRAN",
            OrganizationId = organizationId
        });
        panelTranTrio.AddTest(probandTranTest)
                   .AddTest(additionalFamilyMemberTranTestOne)
                   .AddTest(additionalFamilyMemberTranTestTwo)
                   .Activate();

        var additionalFamilyMemberTranTestThree = Test.Create(new TestForCreation()
        {
            TestCode = "TTRAN004",
            TestName = "Transcriptome Sequencing (Additional Family Member 3)",
            OrganizationId = organizationId,
            Methodology = "Transcriptome Sequencing"
        }).Activate();

        var panelTranQuartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "TRAN001",
            PanelName = "Transcriptome Sequencing Quartet",
            Type = "TRAN",
            OrganizationId = organizationId
        });
        panelTranQuartet.AddTest(probandTranTest)
                     .AddTest(additionalFamilyMemberTranTestOne)
                     .AddTest(additionalFamilyMemberTranTestTwo)
                     .AddTest(additionalFamilyMemberTranTestThree)
                     .Activate();

        await dbContext.Panels.AddRangeAsync(panelTranProband, panelTranDuo, panelTranTrio, panelTranQuartet);

        return new PanelTestResponse()
        {
            Panels = [panelTranProband, panelTranDuo, panelTranTrio, panelTranQuartet],
        };
    }

    private async Task<PanelTestResponse> AddCombinedSequencing(Guid organizationId)
    {
        var probandCombTest = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB001",
            TestName = "Combined Sequencing (Proband)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM, WGS & Transcriptome"
        }).Activate();

        var panelCombProband = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB001",
            PanelName = "Combined Sequencing Proband",
            Type = "COMB",
            OrganizationId = organizationId
        });
        panelCombProband.AddTest(probandCombTest).Activate();

        var additionalFamilyMemberCombTestOne = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB002",
            TestName = "Combined Sequencing (Additional Family Member 1)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM, WGS & Transcriptome"
        }).Activate();

        var panelCombDuo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB001",
            PanelName = "Combined Sequencing Duo",
            Type = "COMB",
            OrganizationId = organizationId
        });
        panelCombDuo.AddTest(probandCombTest)
                   .AddTest(additionalFamilyMemberCombTestOne)
                   .Activate();

        var additionalFamilyMemberCombTestTwo = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB003",
            TestName = "Combined Sequencing (Additional Family Member 2)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM, WGS & Transcriptome"
        }).Activate();

        var panelCombTrio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB001",
            PanelName = "Combined Sequencing Trio",
            Type = "COMB",
            OrganizationId = organizationId
        });
        panelCombTrio.AddTest(probandCombTest)
                   .AddTest(additionalFamilyMemberCombTestOne)
                   .AddTest(additionalFamilyMemberCombTestTwo)
                   .Activate();

        var additionalFamilyMemberCombTestThree = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB004",
            TestName = "Combined Sequencing (Additional Family Member 3)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM, WGS & Transcriptome"
        }).Activate();

        var panelCombQuartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB001",
            PanelName = "Combined Sequencing Quartet",
            Type = "COMB",
            OrganizationId = organizationId
        });
        panelCombQuartet.AddTest(probandCombTest)
                       .AddTest(additionalFamilyMemberCombTestOne)
                       .AddTest(additionalFamilyMemberCombTestTwo)
                       .AddTest(additionalFamilyMemberCombTestThree)
                       .Activate();

        await dbContext.Panels.AddRangeAsync(panelCombProband, panelCombDuo, panelCombTrio, panelCombQuartet);

        return new PanelTestResponse()
        {
            Panels = [panelCombProband, panelCombDuo, panelCombTrio, panelCombQuartet],
        };
    }

    private async Task<PanelTestResponse> AddCombinedOgmWgsSequencing(Guid organizationId)
    {
        var probandComb1Test = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB101",
            TestName = "Combined OGM & WGS Sequencing (Proband)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM & WGS Sequencing"
        }).Activate();

        var panelComb1Proband = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB101",
            PanelName = "Combined OGM & WGS Sequencing Proband",
            Type = "COMB1",
            OrganizationId = organizationId
        });
        panelComb1Proband.AddTest(probandComb1Test).Activate();

        var additionalFamilyMemberComb1TestOne = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB102",
            TestName = "Combined OGM & WGS Sequencing (Additional Family Member 1)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM & WGS Sequencing"
        }).Activate();

        var panelComb1Duo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB101",
            PanelName = "Combined OGM & WGS Sequencing Duo",
            Type = "COMB1",
            OrganizationId = organizationId
        });
        panelComb1Duo.AddTest(probandComb1Test)
                   .AddTest(additionalFamilyMemberComb1TestOne)
                   .Activate();

        var additionalFamilyMemberComb1TestTwo = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB103",
            TestName = "Combined OGM & WGS Sequencing (Additional Family Member 2)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM & WGS Sequencing"
        }).Activate();

        var panelComb1Trio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB101",
            PanelName = "Combined OGM & WGS Sequencing Trio",
            Type = "COMB1",
            OrganizationId = organizationId
        });
        panelComb1Trio.AddTest(probandComb1Test)
                   .AddTest(additionalFamilyMemberComb1TestOne)
                   .AddTest(additionalFamilyMemberComb1TestTwo)
                   .Activate();

        var additionalFamilyMemberComb1TestThree = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB104",
            TestName = "Combined OGM & WGS Sequencing (Additional Family Member 3)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM & WGS Sequencing"
        }).Activate();

        var panelComb1Quartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB101",
            PanelName = "Combined OGM & WGS Sequencing Quartet",
            Type = "COMB1",
            OrganizationId = organizationId
        });
        panelComb1Quartet.AddTest(probandComb1Test)
                       .AddTest(additionalFamilyMemberComb1TestOne)
                       .AddTest(additionalFamilyMemberComb1TestTwo)
                       .AddTest(additionalFamilyMemberComb1TestThree)
                       .Activate();

        await dbContext.Panels.AddRangeAsync(panelComb1Proband, panelComb1Duo, panelComb1Trio, panelComb1Quartet);

        return new PanelTestResponse()
        {
            Panels = [panelComb1Proband, panelComb1Duo, panelComb1Trio, panelComb1Quartet],
        };
    }
    
    private async Task<PanelTestResponse> AddCombinedOgmExternalDataAnalysis(Guid organizationId)
    {
        var probandComb9Test = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB901",
            TestName = "Combined OGM & External Data Analysis (Proband)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM & External Data Analysis"
        }).Activate();

        var panelComb9Proband = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB901",
            PanelName = "Combined OGM & External Data Analysis Proband",
            Type = "COMB9",
            OrganizationId = organizationId
        });
        panelComb9Proband.AddTest(probandComb9Test).Activate();

        var additionalFamilyMemberComb9TestOne = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB902",
            TestName = "Combined OGM & External Data Analysis (Additional Family Member 1)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM & External Data Analysis"
        }).Activate();

        var panelComb9Duo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB901",
            PanelName = "Combined OGM & External Data Analysis Duo",
            Type = "COMB9",
            OrganizationId = organizationId
        });
        panelComb9Duo.AddTest(probandComb9Test)
                   .AddTest(additionalFamilyMemberComb9TestOne)
                   .Activate();

        var additionalFamilyMemberComb9TestTwo = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB903",
            TestName = "Combined OGM & External Data Analysis (Additional Family Member 2)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM & External Data Analysis"
        }).Activate();

        var panelComb9Trio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB901",
            PanelName = "Combined OGM & External Data Analysis Trio",
            Type = "COMB9",
            OrganizationId = organizationId
        });
        panelComb9Trio.AddTest(probandComb9Test)
                   .AddTest(additionalFamilyMemberComb9TestOne)
                   .AddTest(additionalFamilyMemberComb9TestTwo)
                   .Activate();

        var additionalFamilyMemberComb9TestThree = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB904",
            TestName = "Combined OGM & External Data Analysis (Additional Family Member 3)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM & External Data Analysis"
        }).Activate();

        var panelComb9Quartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB901",
            PanelName = "Combined OGM & External Data Analysis Quartet",
            Type = "COMB9",
            OrganizationId = organizationId
        });
        panelComb9Quartet.AddTest(probandComb9Test)
                       .AddTest(additionalFamilyMemberComb9TestOne)
                       .AddTest(additionalFamilyMemberComb9TestTwo)
                       .AddTest(additionalFamilyMemberComb9TestThree)
                       .Activate();

        await dbContext.Panels.AddRangeAsync(panelComb9Proband, panelComb9Duo, panelComb9Trio, panelComb9Quartet);

        return new PanelTestResponse()
        {
            Panels = [panelComb9Proband, panelComb9Duo, panelComb9Trio, panelComb9Quartet],
        };
    }

    private async Task<PanelTestResponse> AddBasicExomeSequencing(Guid organizationId)
    {
        var probandTexTest = Test.Create(new TestForCreation()
        {
            TestCode = "TEX001",
            TestName = "Basic Exome Sequencing (Proband)",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();

        var probandTexGTest = Test.Create(new TestForCreation()
        {
            TestCode = "TEXG001",
            TestName = "Basic Exome Genome Upgrade (Proband)",
            OrganizationId = organizationId,
            Methodology = "Whole Genome Sequencing"
        }).Activate();
        dbContext.Tests.Add(probandTexGTest);

        var panelExProband = Panel.Create(new PanelForCreation()
        {
            PanelCode = "EX001",
            PanelName = "Basic Exome Sequencing Proband",
            Type = "EX",
            OrganizationId = organizationId
        });
        panelExProband.AddTest(probandTexTest).Activate();

        var additionalFamilyMemberTexTestOne = Test.Create(new TestForCreation()
        {
            TestCode = "TEX002",
            TestName = "Basic Exome Sequencing (Additional Family Member 1)",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();

        var duoTexGTest = Test.Create(new TestForCreation()
        {
            TestCode = "TEXG002",
            TestName = "Basic Exome Genome Upgrade (Duo)",
            OrganizationId = organizationId,
            Methodology = "Whole Genome Sequencing"
        }).Activate();
        dbContext.Tests.Add(duoTexGTest);

        var panelExDuo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "EX002",
            PanelName = "Basic Exome Sequencing Duo",
            Type = "EX",
            OrganizationId = organizationId
        });
        panelExDuo.AddTest(probandTexTest)
                 .AddTest(additionalFamilyMemberTexTestOne)
                 .Activate();

        var additionalFamilyMemberTexTestTwo = Test.Create(new TestForCreation()
        {
            TestCode = "TEX003",
            TestName = "Basic Exome Sequencing (Additional Family Member 2)",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();

        var trioTexGTest = Test.Create(new TestForCreation()
        {
            TestCode = "TEXG003",
            TestName = "Basic Exome Genome Upgrade (Trio)",
            OrganizationId = organizationId,
            Methodology = "Whole Genome Sequencing"
        }).Activate();
        dbContext.Tests.Add(trioTexGTest);

        var panelExTrio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "EX003",
            PanelName = "Basic Exome Sequencing Trio",
            Type = "EX",
            OrganizationId = organizationId
        });
        panelExTrio.AddTest(probandTexTest)
                  .AddTest(additionalFamilyMemberTexTestOne)
                  .AddTest(additionalFamilyMemberTexTestTwo)
                  .Activate();

        var additionalFamilyMemberTexTestThree = Test.Create(new TestForCreation()
        {
            TestCode = "TEX004",
            TestName = "Basic Exome Sequencing (Additional Family Member 3)",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();

        var quartetTexGTest = Test.Create(new TestForCreation()
        {
            TestCode = "TEXG004",
            TestName = "Basic Exome Genome Upgrade (Quartet)",
            OrganizationId = organizationId,
            Methodology = "Whole Genome Sequencing"
        }).Activate();
        dbContext.Tests.Add(quartetTexGTest);

        var panelExQuartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "EX004",
            PanelName = "Basic Exome Sequencing Quartet",
            Type = "EX",
            OrganizationId = organizationId
        });
        panelExQuartet.AddTest(probandTexTest)
                    .AddTest(additionalFamilyMemberTexTestOne)
                    .AddTest(additionalFamilyMemberTexTestTwo)
                    .AddTest(additionalFamilyMemberTexTestThree)
                    .Activate();

        await dbContext.Panels.AddRangeAsync(panelExProband, panelExDuo, panelExTrio, panelExQuartet);
        return new PanelTestResponse()
        {
            Panels = [panelExProband, panelExDuo, panelExTrio, panelExQuartet],
        };
    }

    private async Task<PanelTestResponse> AddExpandedExomeSequencing(Guid organizationId)
    {
        var probandTeexTest = Test.Create(new TestForCreation()
        {
            TestCode = "TEEX001",
            TestName = "Expanded Exome Sequencing (Proband)",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();

        var probandTeexGTest = Test.Create(new TestForCreation()
        {
            TestCode = "TEEXG001",
            TestName = "Expanded Exome Genome Upgrade (Proband)",
            OrganizationId = organizationId,
            Methodology = "Whole Genome Sequencing"
        }).Activate();
        dbContext.Tests.Add(probandTeexGTest);

        var panelEexProband = Panel.Create(new PanelForCreation()
        {
            PanelCode = "EEX001",
            PanelName = "Expanded Exome Sequencing Proband",
            Type = "EEX",
            OrganizationId = organizationId
        });
        panelEexProband.AddTest(probandTeexTest).Activate();

        var additionalFamilyMemberTeexTestOne = Test.Create(new TestForCreation()
        {
            TestCode = "TEEX002",
            TestName = "Expanded Exome Sequencing (Additional Family Member 1)",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();

        var duoTeexGTest = Test.Create(new TestForCreation()
        {
            TestCode = "TEEXG002",
            TestName = "Expanded Exome Genome Upgrade (Duo)",
            OrganizationId = organizationId,
            Methodology = "Whole Genome Sequencing"
        }).Activate();
        dbContext.Tests.Add(duoTeexGTest);

        var panelEexDuo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "EEX002",
            PanelName = "Expanded Exome Sequencing Duo",
            Type = "EEX",
            OrganizationId = organizationId
        });
        panelEexDuo.AddTest(probandTeexTest)
                  .AddTest(additionalFamilyMemberTeexTestOne)
                  .Activate();

        var additionalFamilyMemberTeexTestTwo = Test.Create(new TestForCreation()
        {
            TestCode = "TEEX003",
            TestName = "Expanded Exome Sequencing (Additional Family Member 2)",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();

        var trioTeexGTest = Test.Create(new TestForCreation()
        {
            TestCode = "TEEXG003",
            TestName = "Expanded Exome Genome Upgrade (Trio)",
            OrganizationId = organizationId,
            Methodology = "Whole Genome Sequencing"
        }).Activate();
        dbContext.Tests.Add(trioTeexGTest);

        var panelEexTrio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "EEX003",
            PanelName = "Expanded Exome Sequencing Trio",
            Type = "EEX",
            OrganizationId = organizationId
        });
        panelEexTrio.AddTest(probandTeexTest)
                   .AddTest(additionalFamilyMemberTeexTestOne)
                   .AddTest(additionalFamilyMemberTeexTestTwo)
                   .Activate();

        var additionalFamilyMemberTeexTestThree = Test.Create(new TestForCreation()
        {
            TestCode = "TEEX004",
            TestName = "Expanded Exome Sequencing (Additional Family Member 3)",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();

        var quartetTeexGTest = Test.Create(new TestForCreation()
        {
            TestCode = "TEEXG004",
            TestName = "Expanded Exome Genome Upgrade (Quartet)",
            OrganizationId = organizationId,
            Methodology = "Whole Genome Sequencing"
        }).Activate();
        dbContext.Tests.Add(quartetTeexGTest);

        var panelEexQuartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "EEX004",
            PanelName = "Expanded Exome Sequencing Quartet",
            Type = "EEX",
            OrganizationId = organizationId
        });
        panelEexQuartet.AddTest(probandTeexTest)
                     .AddTest(additionalFamilyMemberTeexTestOne)
                     .AddTest(additionalFamilyMemberTeexTestTwo)
                     .AddTest(additionalFamilyMemberTeexTestThree)
                     .Activate();

        await dbContext.Panels.AddRangeAsync(panelEexProband, panelEexDuo, panelEexTrio, panelEexQuartet);

        return new PanelTestResponse()
        {
            Panels = [panelEexProband, panelEexDuo, panelEexTrio, panelEexQuartet],
        };
    }
}