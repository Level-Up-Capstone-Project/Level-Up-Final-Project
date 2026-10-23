using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Profile;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CapstoneProjectBE.Services
{
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _context;

        public ProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken)
        {
            UserProfile? profile = await _context.UserProfiles.AsNoTracking().SingleOrDefaultAsync(profile => profile.UserID == userId, cancellationToken);
            return profile is null ? null : MapToDto(profile);
        }

        public async Task<ProfileDto?> UpsertProfileAsync(Guid userId, UpdateProfileDto request, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(request.FName) ||  string.IsNullOrWhiteSpace(request.LName) || string.IsNullOrWhiteSpace(request.Qualification) || string.IsNullOrWhiteSpace(request.FieldOfStudy))
            {
                return null;
            }

            bool userExists = await _context.Users.AnyAsync(user => user.UserID == userId, cancellationToken);

            if(!userExists)
            {
                return null;
            }

            if(request.TargetCareerID.HasValue)
            {
                bool careerExists = await _context.Careers.AnyAsync(career => career.CareerID == request.TargetCareerID.Value, cancellationToken);

                if(!careerExists)
                {
                    return null;
                }
            }

            UserProfile? profile = await _context.UserProfiles.SingleOrDefaultAsync(profile => profile.UserID == userId, cancellationToken);

            if(profile is null)
            {
                profile = new UserProfile
                {
                    ProfileID = Guid.NewGuid(),
                    UserID = userId
                };

                _context.UserProfiles.Add(profile);
            }

            profile.FName = request.FName.Trim();
            profile.LName = request.LName.Trim();
            profile.Qualification = request.Qualification.Trim();
            profile.FieldOfStudy = request.FieldOfStudy.Trim();
            profile.ExperienceLevel = NormalizeOptionalValue(request.ExperienceLevel);
            profile.Interests = NormalizeOptionalValue(request.Interests);
            profile.Industry = request.Industry is null ? profile.Industry : NormalizeOptionalValue(request.Industry);
            profile.Phone = request.Phone is null ? profile.Phone : NormalizeOptionalValue(request.Phone);
            if (request.TargetRoles is not null) profile.TargetRoles = NormalizeTargetRoles(request.TargetRoles);
            if (request.WorkAreas is not null) profile.WorkAreasJson = JsonSerializer.Serialize(request.WorkAreas);
            profile.TargetCareerID = request.TargetCareerID;
            profile.UpdatedAt = DateTime.UtcNow;
            profile.ProfileCompleted = IsProfileComplete(profile);

            await _context.SaveChangesAsync(cancellationToken);
            return MapToDto(profile);
        }

        public async Task<ProfileDto?> UpdatePreferencesAsync(Guid userId, UpdateProfilePreferencesDto request, CancellationToken cancellationToken)
        {
            UserProfile? profile = await _context.UserProfiles.SingleOrDefaultAsync(item => item.UserID == userId, cancellationToken);
            if (profile is null)
            {
                if (!await _context.Users.AnyAsync(user => user.UserID == userId && user.IsActive, cancellationToken)) return null;
                profile = new UserProfile {ProfileID = Guid.NewGuid(), UserID = userId};
                _context.UserProfiles.Add(profile);
            }

            profile.Industry = NormalizeOptionalValue(request.Industry);
            profile.Phone = NormalizeOptionalValue(request.Phone);
            profile.TargetRoles = NormalizeTargetRoles(request.TargetRoles ?? new());
            profile.WorkAreasJson = JsonSerializer.Serialize(request.WorkAreas);
            profile.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return MapToDto(profile);
        }

        public async Task<ProfileDto?> UpdateTargetCareerAsync(Guid userId, Guid careerId, CancellationToken cancellationToken)
        {
            if(careerId ==  Guid.Empty)
            {
                return null;
            }

            bool careerExists = await _context.Careers.AnyAsync(career => career.CareerID == careerId, cancellationToken);

            if(!careerExists)
            {
                return null;
            }

            UserProfile? profile = await _context.UserProfiles.SingleOrDefaultAsync(profile => profile.UserID == userId, cancellationToken);

            if(profile is null)
            {
                if (!await _context.Users.AnyAsync(user => user.UserID == userId && user.IsActive, cancellationToken)) return null;
                // A selected career can be saved before the rest of onboarding is complete.
                profile = new UserProfile {ProfileID = Guid.NewGuid(), UserID = userId};
                _context.UserProfiles.Add(profile);
            }

            profile.TargetCareerID = careerId;
            profile.ProfileCompleted = IsProfileComplete(profile);
            profile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return MapToDto(profile);
        }

        private static bool IsProfileComplete(UserProfile profile)
        {
            return !string.IsNullOrWhiteSpace(profile.FName) && !string.IsNullOrWhiteSpace(profile.LName) && !string.IsNullOrWhiteSpace(profile.Qualification) && !string.IsNullOrWhiteSpace(profile.FieldOfStudy) && profile.TargetCareerID.HasValue;
        }

        private static string? NormalizeOptionalValue(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static string[] NormalizeTargetRoles(IEnumerable<string> values) =>
            values.Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

        private static ProfileDto MapToDto(UserProfile profile)
        {
            return new ProfileDto
            {
                ProfileID = profile.ProfileID,
                UserID = profile.UserID,
                TargetCareerID = profile.TargetCareerID,
                FName = profile.FName,
                LName = profile.LName,
                Qualification = profile.Qualification,
                FieldOfStudy = profile.FieldOfStudy,
                ExperienceLevel = profile.ExperienceLevel,
                Interests = profile.Interests,
                Industry = profile.Industry,
                Phone = profile.Phone,
                TargetRoles = profile.TargetRoles,
                WorkAreas = string.IsNullOrWhiteSpace(profile.WorkAreasJson)
                    ? Array.Empty<WorkAreaDto>()
                    : JsonSerializer.Deserialize<List<WorkAreaDto>>(profile.WorkAreasJson) ?? new(),
                ProfileCompleted = profile.ProfileCompleted,
                UpdatedAt = profile.UpdatedAt
            };
        }
    }
}
