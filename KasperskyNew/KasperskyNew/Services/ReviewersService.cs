using KasperskyNew.Models;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace KasperskyNew.Services
{
    public interface IReviewersService
    {
        public Task<List<string>> GetReviewersAsync(List<string> paths, string configPath);

    }

    public class ReviewersService : IReviewersService
    {
        private readonly IDeserializer _yamlDeserializer;

        public ReviewersService()
        {
            _yamlDeserializer = new DeserializerBuilder().Build();
        }

        public async Task<List<string>> GetReviewersAsync(List<string> paths, string configPath)
        {
            if (!File.Exists(configPath))
                throw new FileNotFoundException("Config file not found.");

            var configContent = await File.ReadAllTextAsync(configPath);
            var config = _yamlDeserializer.Deserialize<ReviewersConfig>(configContent);

            var reviewers = new HashSet<string>();

            foreach (var rule in config.Rules.Values)
            {
                foreach (var path in paths)
                {
                    if (IsPathIncluded(rule, path) && !IsPathExcluded(rule, path))
                    {
                        foreach (var reviewer in rule.Reviewers)
                        {
                            reviewers.Add(reviewer);
                        }
                    }
                }
            }

            return reviewers.ToList();
        }

        private bool IsPathIncluded(ReviewRule rule, string path)
        {
            return rule.IncludedPaths.Any(pattern => PathMatchesPattern(path, pattern));
        }

        private bool IsPathExcluded(ReviewRule rule, string path)
        {
            return rule.ExcludedPaths.Any(pattern => PathMatchesPattern(path, pattern));
        }

        private bool PathMatchesPattern(string path, string pattern)
        {
            var regexPattern = "^" + Regex.Escape(pattern)
                                .Replace(@"\*\*", ".*")
                                .Replace(@"\*", @"[^/]*")
                                .Replace(@"\?", ".") + "$";
            return Regex.IsMatch(path, regexPattern);
        }
    }


}
