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
            TestName = "Optical Genome Mapping Test (Proband)",
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
        
        
        var additionalFamilyMemberOgmTest = Test.Create(new TestForCreation()
        {
            TestCode = "TOGM002",
            TestName = "Optical Genome Mapping (Additional Family Member)",
            OrganizationId = organizationId,
            Methodology = "Optical Genome Mapping"
        }).Activate();
        var panelOgmDuo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "OGM002",
            PanelName = "Optical Genome Mapping Duo",
            Type = "OGM",
            OrganizationId = organizationId
        });
        panelOgmDuo.AddTest(probandOgmTest)
            .AddTest(additionalFamilyMemberOgmTest)
            .Activate();
        
        var panelOgmTrio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "OGM003",
            PanelName = "Optical Genome Mapping Trio",
            Type = "OGM",
            OrganizationId = organizationId
        });
        panelOgmTrio.AddTest(probandOgmTest)
            .AddTest(additionalFamilyMemberOgmTest, 2)
            .Activate();
        
        var panelOgmQuartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "OGM004",
            PanelName = "Optical Genome Mapping Quartet",
            Type = "OGM",
            OrganizationId = organizationId
        });
        panelOgmQuartet.AddTest(probandOgmTest)
            .AddTest(additionalFamilyMemberOgmTest, 3)
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
            TestName = "Transcriptome Sequencing Test (Proband)",
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

        var additionalFamilyMemberTranTest = Test.Create(new TestForCreation()
        {
            TestCode = "TTRAN002",
            TestName = "Transcriptome Sequencing (Additional Family Member)",
            OrganizationId = organizationId,
            Methodology = "Transcriptome Sequencing"
        }).Activate();

        var panelTranDuo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "TRAN002",
            PanelName = "Transcriptome Sequencing Duo",
            Type = "TRAN",
            OrganizationId = organizationId
        });
        panelTranDuo.AddTest(probandTranTest)
                   .AddTest(additionalFamilyMemberTranTest)
                   .Activate();

        var panelTranTrio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "TRAN003",
            PanelName = "Transcriptome Sequencing Trio",
            Type = "TRAN",
            OrganizationId = organizationId
        });
        panelTranTrio.AddTest(probandTranTest)
                   .AddTest(additionalFamilyMemberTranTest, 2)
                   .Activate();

        var panelTranQuartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "TRAN004",
            PanelName = "Transcriptome Sequencing Quartet",
            Type = "TRAN",
            OrganizationId = organizationId
        });
        panelTranQuartet.AddTest(probandTranTest)
                     .AddTest(additionalFamilyMemberTranTest, 3)
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
            TestName = "Combined Sequencing Test (Proband)",
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

        var additionalFamilyMemberCombTest = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB002",
            TestName = "Combined Sequencing (Additional Family Member)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM, WGS & Transcriptome"
        }).Activate();

        var panelCombDuo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB002",
            PanelName = "Combined Sequencing Duo",
            Type = "COMB",
            OrganizationId = organizationId
        });
        panelCombDuo.AddTest(probandCombTest)
                   .AddTest(additionalFamilyMemberCombTest)
                   .Activate();

        var panelCombTrio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB003",
            PanelName = "Combined Sequencing Trio",
            Type = "COMB",
            OrganizationId = organizationId
        });
        panelCombTrio.AddTest(probandCombTest)
                   .AddTest(additionalFamilyMemberCombTest, 2)
                   .Activate();

        var panelCombQuartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB004",
            PanelName = "Combined Sequencing Quartet",
            Type = "COMB",
            OrganizationId = organizationId
        });
        panelCombQuartet.AddTest(probandCombTest)
                       .AddTest(additionalFamilyMemberCombTest, 3)
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
            TestName = "Combined OGM & WGS Sequencing Test (Proband)",
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

        var additionalFamilyMemberComb1Test = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB102",
            TestName = "Combined OGM & WGS Sequencing (Additional Family Member)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM & WGS Sequencing"
        }).Activate();

        var panelComb1Duo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB102",
            PanelName = "Combined OGM & WGS Sequencing Duo",
            Type = "COMB1",
            OrganizationId = organizationId
        });
        panelComb1Duo.AddTest(probandComb1Test)
                   .AddTest(additionalFamilyMemberComb1Test)
                   .Activate();

        var panelComb1Trio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB103",
            PanelName = "Combined OGM & WGS Sequencing Trio",
            Type = "COMB1",
            OrganizationId = organizationId
        });
        panelComb1Trio.AddTest(probandComb1Test)
                   .AddTest(additionalFamilyMemberComb1Test, 2)
                   .Activate();

        var panelComb1Quartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB101",
            PanelName = "Combined OGM & WGS Sequencing Quartet",
            Type = "COMB4",
            OrganizationId = organizationId
        });
        panelComb1Quartet.AddTest(probandComb1Test)
                       .AddTest(additionalFamilyMemberComb1Test, 3)
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
            TestName = "Combined OGM & External Data Analysis Test (Proband)",
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

        var additionalFamilyMemberComb9Test = Test.Create(new TestForCreation()
        {
            TestCode = "TCOMB902",
            TestName = "Combined OGM & External Data Analysis (Additional Family Member)",
            OrganizationId = organizationId,
            Methodology = "Combined OGM & External Data Analysis"
        }).Activate();

        var panelComb9Duo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB902",
            PanelName = "Combined OGM & External Data Analysis Duo",
            Type = "COMB9",
            OrganizationId = organizationId
        });
        panelComb9Duo.AddTest(probandComb9Test)
                   .AddTest(additionalFamilyMemberComb9Test)
                   .Activate();

        var panelComb9Trio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB903",
            PanelName = "Combined OGM & External Data Analysis Trio",
            Type = "COMB9",
            OrganizationId = organizationId
        });
        panelComb9Trio.AddTest(probandComb9Test)
                   .AddTest(additionalFamilyMemberComb9Test, 2)
                   .Activate();

        var panelComb9Quartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "COMB904",
            PanelName = "Combined OGM & External Data Analysis Quartet",
            Type = "COMB9",
            OrganizationId = organizationId
        });
        panelComb9Quartet.AddTest(probandComb9Test)
                       .AddTest(additionalFamilyMemberComb9Test, 3)
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
            TestName = "Basic Exome Sequencing Test (Proband)",
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

        var additionalFamilyMemberTexTest = Test.Create(new TestForCreation()
        {
            TestCode = "TEX002",
            TestName = "Basic Exome Sequencing (Additional Family Member)",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();

        var panelExDuo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "EX002",
            PanelName = "Basic Exome Sequencing Duo",
            Type = "EX",
            OrganizationId = organizationId
        });
        panelExDuo.AddTest(probandTexTest)
                 .AddTest(additionalFamilyMemberTexTest)
                 .Activate();

        var panelExTrio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "EX003",
            PanelName = "Basic Exome Sequencing Trio",
            Type = "EX",
            OrganizationId = organizationId
        });
        panelExTrio.AddTest(probandTexTest)
                  .AddTest(additionalFamilyMemberTexTest, 2)
                  .Activate();

        var panelExQuartet = Panel.Create(new PanelForCreation()
        {
            PanelCode = "EX004",
            PanelName = "Basic Exome Sequencing Quartet",
            Type = "EX",
            OrganizationId = organizationId
        });
        panelExQuartet.AddTest(probandTexTest)
                    .AddTest(additionalFamilyMemberTexTest, 3)
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
            TestName = "Expanded Exome Sequencing Test (Proband)",
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

        var additionalFamilyMemberTeexTest = Test.Create(new TestForCreation()
        {
            TestCode = "TEEX002",
            TestName = "Expanded Exome Sequencing (Additional Family Member)",
            OrganizationId = organizationId,
            Methodology = "Exome Sequencing"
        }).Activate();

        var panelEexDuo = Panel.Create(new PanelForCreation()
        {
            PanelCode = "EEX002",
            PanelName = "Expanded Exome Sequencing Duo",
            Type = "EEX",
            OrganizationId = organizationId
        });
        panelEexDuo.AddTest(probandTeexTest)
                  .AddTest(additionalFamilyMemberTeexTest)
                  .Activate();

        var panelEexTrio = Panel.Create(new PanelForCreation()
        {
            PanelCode = "EEX003",
            PanelName = "Expanded Exome Sequencing Trio",
            Type = "EEX",
            OrganizationId = organizationId
        });
        panelEexTrio.AddTest(probandTeexTest)
                   .AddTest(additionalFamilyMemberTeexTest, 2)
                   .Activate();

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
                     .AddTest(additionalFamilyMemberTeexTest, 3)
                     .Activate();

        await dbContext.Panels.AddRangeAsync(panelEexProband, panelEexDuo, panelEexTrio, panelEexQuartet);

        return new PanelTestResponse()
        {
            Panels = [panelEexProband, panelEexDuo, panelEexTrio, panelEexQuartet],
        };
    }
}