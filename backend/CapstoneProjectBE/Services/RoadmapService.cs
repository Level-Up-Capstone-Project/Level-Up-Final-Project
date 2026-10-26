using CapstoneProjectBE.Services.Interfaces;
using CapstoneProjectBE.DTOs.Roadmaps;
using CapstoneProjectBE.DTOs.Resources;
using CapstoneProjectBE.Data;
using Microsoft.EntityFrameworkCore;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.ExternalServices.Roadmaps;


namespace CapstoneProjectBE.Services
{
    /// <summary>
    /// Coordinates Roadmap service application behavior.
    /// </summary>
    public class RoadmapService : IRoadmapService
    {
        //roadmaps, steps, and resource progress
        //completing resources updates progress, does not verify skill
        private readonly AppDbContext _context;
        private readonly N8nRoadmapGenerator _generator;

        public RoadmapService(AppDbContext context, N8nRoadmapGenerator generator)
        {
            _context = context;
            _generator = generator;
        }
        
        // GET /api/roadmaps/items/{id}/resources
        public async Task<List<RoadmapItemResourceDto>?> GetItemResourceAsync(Guid userID, Guid roadmapItemID)
        {
            bool itemExists = await _context.RoadmapItems
                .AnyAsync(item => item.RoadmapItemID == roadmapItemID
                    && item.Roadmap.UserID == userID);

            if (!itemExists)
            {
                return null;
            }

            return await _context.RoadmapItemResources
                .AsNoTracking()
                .Where(link => link.RoadmapItemID == roadmapItemID
                    && link.RoadmapItem.Roadmap.UserID == userID)
                .OrderBy(link => link.LearningResource.ResourceName)
                .Select(link => new RoadmapItemResourceDto
                {
                    RoadmapItemResourceID = link.RoadmapItemResourceID,
                    IsCompleted = link.IsCompleted,
                    CompletedAt = link.CompletedAt,
                    Resource = new LearningResourceDto
                    {
                        ResourceID = link.LearningResource.ResourceID,
                        SkillID = link.LearningResource.SkillID,
                        ResourceName = link.LearningResource.ResourceName,
                        ResourceType = link.LearningResource.ResourceType,
                        Url = link.LearningResource.Url,
                        Provider = link.LearningResource.Provider,
                        DifficultyLevel = link.LearningResource.DifficultyLevel,
                        CostType = link.LearningResource.CostType,
                        EstDuration = link.LearningResource.EstDuration,
                        Description = link.LearningResource.Description
                    }
                })
                .ToListAsync();
        }

        //GET /api/roadmaps/items/{id}
        public async Task<RoadmapItemDto?> GetItemByIdAsync(Guid userID, Guid roadmapItemID)
        {
            return await _context.RoadmapItems
                .AsNoTracking()
                .Where(item => item.RoadmapItemID == roadmapItemID
                    && item.Roadmap.UserID == userID)
                .Select(item => new RoadmapItemDto
                {
                    RoadmapItemID = item.RoadmapItemID,
                    RoadmapID = item.RoadmapID,
                    SkillID = item.SkillID,
                    SkillName = item.Skill.SkillName,
                    Title = item.Title,
                    Description = item.Description,
                    StepOrder = item.StepOrder,
                    Priority = item.Priority,
                    Status = item.Status,
                    TargetDate = item.TargetDate,
                    CompletedAt = item.CompletedAt
                })
                .FirstOrDefaultAsync();
        }

        //GET /api/roadmaps/{id}
        public async Task<RoadmapDto?> GetRoadmapByIdAsync(Guid userID, Guid roadmapID)
        {
            return await _context.LearningRoadmaps
            .AsNoTracking()
            .Where(roadmap => roadmap.RoadmapID == roadmapID && roadmap.UserID == userID)
            .Select(roadmap => new RoadmapDto
            {
                RoadmapID = roadmap.RoadmapID,
                UserID = roadmap.UserID,
                CareerID = roadmap.CareerID,
                CareerName = roadmap.Career.CareerName,
                Title = roadmap.Title,
                ProgressPercentage = roadmap.ProgressPercentage,
                CreatedAt = roadmap.CreatedAt,
                UpdatedAt = roadmap.UpdatedAt,

                //steps in order
                Items = roadmap.Items
                .OrderBy(item => item.StepOrder)
                .Select(item => new RoadmapItemDto
                {
                    RoadmapItemID = item.RoadmapItemID,
                    RoadmapID = item.RoadmapID,
                    SkillID = item.SkillID,
                    SkillName = item.Skill.SkillName,
                    Title = item.Title,
                    Description = item.Description,
                    StepOrder = item.StepOrder,
                    Priority = item.Priority,
                    Status = item.Status,
                    TargetDate = item.TargetDate,
                    CompletedAt = item.CompletedAt
                })
                .ToList()
            })
            .FirstOrDefaultAsync();
        }

        //GET /api/roadmaps
        public async Task<List<RoadmapSummaryDto>> GetRoadmapForUserAsync(Guid userID)
        {
            return await _context.LearningRoadmaps
            .AsNoTracking()
            .Where(roadmap => roadmap.UserID == userID)
            .OrderByDescending(roadmap => roadmap.UpdatedAt)
            .Select(roadmap => new RoadmapSummaryDto
            {
                RoadmapID = roadmap.RoadmapID,
                CareerID = roadmap.CareerID,
                CareerName = roadmap.Career.CareerName,
                Title = roadmap.Title,
                ProgressPercentage = roadmap.ProgressPercentage,
                CreatedAt = roadmap.CreatedAt,
                UpdatedAt = roadmap.UpdatedAt
            })
            .ToListAsync();
            
        }

        //GET /api/roadmaps/current
        public async Task<RoadmapDto?> GetCurrentRoadmapAsync (Guid userID)
        {
            //current = roadmap user worked on most recently
            var currentID = await _context.LearningRoadmaps
            .AsNoTracking()
            .Where(roadmap => roadmap.UserID == userID)
            .OrderByDescending(roadmap => roadmap.UpdatedAt)
            .Select(roadmap => (Guid?)roadmap.RoadmapID)
            .FirstOrDefaultAsync();

            if (currentID == null )
            {
                return null;
            }
            return await GetRoadmapByIdAsync(userID, currentID.Value);
        }

        //POST /api/roadmaps/items/{itemID}/resources/{resourceID}/complete
        //updates progress only, does not verify skill
        public async Task<CompleteResourceResultDto?> CompleteResourceAsync(
            Guid userID,
            Guid roadmapItemID,
            Guid resourceID)
        {
            var link = await _context.RoadmapItemResources
                .Include(link => link.RoadmapItem)
                .FirstOrDefaultAsync(link => link.RoadmapItemID == roadmapItemID
                    && link.ResourceID == resourceID
                    && link.RoadmapItem.Roadmap.UserID == userID);

            if (link == null)
            {
                return null;
            }

            var item = link.RoadmapItem;
            var roadmapID = item.RoadmapID;
            var now = DateTime.UtcNow;

            //mark resource complete
            //keep original date if already done
            if (!link.IsCompleted)
            {
                link.IsCompleted = true;
                link.CompletedAt = now;
                await _context.SaveChangesAsync();
            }

            //step status is completed when all its resources are done
            var anyLeft = await _context.RoadmapItemResources
            .AnyAsync(l => l.RoadmapItemID == item.RoadmapItemID && !l.IsCompleted);

            if (!anyLeft && item.Status != "completed")
            {
                item.Status = "completed";
                item.CompletedAt = now;
            }
             else if (anyLeft && item.Status == "not_started") 
            {
                item.Status = "in_progress";
            }
            
            //update roadmap progress
            //counts all resources across every step in roadmap
            var total = await _context.RoadmapItemResources
            .CountAsync(l =>  l.RoadmapItem.RoadmapID == roadmapID);

            //only counts the ticked ones
            var completed = await _context.RoadmapItemResources
            .CountAsync(l => l.RoadmapItem.RoadmapID == roadmapID && l.IsCompleted);

            var roadmap = await _context.LearningRoadmaps
            .FirstAsync(roadmap => roadmap.RoadmapID == roadmapID);

            roadmap.ProgressPercentage = Math.Round(completed * 100m/total, 2);
            roadmap.UpdatedAt = now;

            //saves step status and roadmap progress together
            await _context.SaveChangesAsync();

            return new CompleteResourceResultDto
            {
                RoadmapItemResourceID = link.RoadmapItemResourceID,
                IsCompleted = link.IsCompleted,
                CompletedAt = link.CompletedAt,
                RoadmapItemID = item.RoadmapItemID,
                ItemStatus = item.Status,
                RoadmapID = roadmapID,
                ProgressPercentage = roadmap.ProgressPercentage
            };

        }

        //POST /api/roadmaps/generate
        public async Task<RoadmapDto?> GenerateRoadmapAsync(Guid userID, Guid careerID, CancellationToken cancellationToken)
        {
            //get career + its skills
            //most important first
            var career = await _context.Careers
            .AsNoTracking()
            .Where(career => career.CareerID == careerID)
            .Select(career => new
            {
                career.CareerName,
                Skills = career.CareerSkills
                .OrderByDescending(cs => cs.IsCoreSkill)
                .ThenByDescending(cs => cs.ImportanceLevel)
                .Select(cs => new { cs.SkillID, cs.Skill.SkillName, cs.Skill.Category, cs.ImportanceLevel, cs.IsCoreSkill })
                .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

            if (career == null)
            {
                return null;
            }

            var userSkillStatuses = await _context.UserSkills
                .AsNoTracking()
                .Where(userSkill => userSkill.UserID == userID)
                .ToDictionaryAsync(userSkill => userSkill.SkillID, userSkill => userSkill.Status, cancellationToken);

            var workflowSkills = career.Skills.Select(skill => new N8nRoadmapSkill(
                skill.SkillID, skill.SkillName, skill.Category, skill.ImportanceLevel, skill.IsCoreSkill,
                userSkillStatuses.TryGetValue(skill.SkillID, out var status) ? UserSkillStatusResolver.ToApiStatus(status) : "Missing"))
                .ToList();
            var roadmapSkills = workflowSkills.Where(skill => skill.Status is "Missing" or "Partial").ToList();

            var skillIDs = roadmapSkills.Select(skill => skill.SkillID).ToList();

            var resources = await _context.LearningResources
            .AsNoTracking()
            .Where(resource => skillIDs.Contains(resource.SkillID) && resource.IsActive)
            .Select(resource => new { resource.ResourceID, resource.SkillID, resource.ResourceName})
            .ToListAsync(cancellationToken);

            GeneratedRoadmap? generated = roadmapSkills.Count == 0
                ? null
                : await _generator.GenerateAsync(careerID, career.CareerName, workflowSkills, cancellationToken);

            var now = DateTime.UtcNow;

            //create the roadmap
            var roadmap = new LearningRoadmap
            {
                UserID = userID,
                CareerID = careerID,
                Title = generated?.Title ?? $"{career.CareerName} Roadmap",
                ProgressPercentage = roadmapSkills.Count == 0 ? 100 : 0,
                CreatedAt = now,
                UpdatedAt = now
            };

            //one step per skill with all its active resources
            var stepOrder = 1;

            foreach(var generatedItem in generated?.Items ?? Array.Empty<GeneratedRoadmapItem>())
            {
                var skill = roadmapSkills.Single(candidate => candidate.SkillID == generatedItem.SkillID);
                var item = new RoadmapItem
                {
                    SkillID = skill.SkillID,
                    Title = generatedItem.Title,
                    Description = generatedItem.Description,
                    StepOrder = stepOrder++,
                    Priority = generatedItem.Priority,
                    Status = "not_started"
                };

                var skillResources = resources
                .Where(resource => resource.SkillID == skill.SkillID)
                .OrderBy(resource => resource.ResourceName)
                .Take(3) //3 resources per step
                .ToList();

                foreach (var resource in skillResources)
                {
                    item.Resources.Add(new RoadmapItemResource
                    {
                        ResourceID = resource.ResourceID,
                        IsCompleted = false
                    });
                }

                foreach (var recommendation in (generated?.LearningResources ?? Array.Empty<GeneratedLearningResource>())
                    .Where(resource => resource.MatchedSkills.Any(name => string.Equals(name, skill.SkillName, StringComparison.OrdinalIgnoreCase)))
                    .DistinctBy(resource => resource.Url, StringComparer.OrdinalIgnoreCase))
                {
                    var existing = await _context.LearningResources.FirstOrDefaultAsync(resource =>
                        resource.SkillID == skill.SkillID && resource.Url == recommendation.Url, cancellationToken);
                    if (existing is not null && skillResources.Any(resource => resource.ResourceID == existing.ResourceID)) continue;
                    item.Resources.Add(new RoadmapItemResource
                    {
                        LearningResource = existing ?? new LearningResource
                        {
                            SkillID = skill.SkillID, ResourceName = recommendation.Title,
                            Provider = recommendation.Provider, Url = recommendation.Url,
                            ResourceType = "Course", CostType = recommendation.AccessType,
                            DifficultyLevel = "", EstDuration = "", Description = "", SourceType = "n8n", IsActive = true
                        },
                        IsCompleted = false
                    });
                }

                roadmap.Items.Add(item);

            }
            //save roadmap, steps, and resource links together
            _context.LearningRoadmaps.Add(roadmap);
            await _context.SaveChangesAsync(cancellationToken);

            return await GetRoadmapByIdAsync(userID, roadmap.RoadmapID);

        }

    }
}
