using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using RabbitMQMonitor.Models;
using Scriban;
using PreMailerDotNet = PreMailer.Net.PreMailer;

namespace RabbitMQMonitor.Services
{
    /// <summary>
    /// Renders the alert mail bodies held as Scriban templates under <c>Templates/</c>.
    /// </summary>
    /// <remarks>
    /// The template files are copied to the output directory by the <c>Content</c> items in
    /// <c>RabbitMQMonitor.csproj</c> — drop those and rendering fails at runtime with a missing file.
    /// Parsed templates are cached, so editing an .html file takes effect only on the next run.
    /// <para>
    /// The templates keep their CSS in a single <c>&lt;style&gt;</c> block for readability; PreMailer
    /// flattens it onto each element before sending, because Gmail's mobile app and several webmail
    /// clients strip <c>&lt;head&gt;</c> entirely and would otherwise render an unstyled mail.
    /// </para>
    /// </remarks>
    public class MailTemplateService
    {
        private static readonly string TemplateDirectory = Path.Combine(AppContext.BaseDirectory, "Templates");

        private readonly ConcurrentDictionary<string, Template> _cache = new();
        private readonly ILogger<MailTemplateService> _logger;

        public MailTemplateService(ILogger<MailTemplateService> logger)
        {
            _logger = logger;
        }

        /// <summary>Renders <paramref name="templateName"/> with <paramref name="data"/> as the model,
        /// then inlines the template's CSS for mail clients that drop <c>&lt;style&gt;</c>.</summary>
        public async Task<string> RenderAsync(string templateName, QueueAlertData data)
        {
            var template = GetTemplate(templateName);
            var html = await template.RenderAsync(data);
            return InlineCss(templateName, html);
        }

        private string InlineCss(string templateName, string html)
        {
            var result = PreMailerDotNet.MoveCssInline(html, removeStyleElements: true);
            if (result.Warnings.Count > 0)
            {
                _logger.LogWarning("PreMailer reported warnings while inlining {templateName}: {warnings}", templateName, string.Join("; ", result.Warnings));
            }
            return result.Html;
        }

        private Template GetTemplate(string templateName) => _cache.GetOrAdd(templateName, name =>
        {
            var path = Path.Combine(TemplateDirectory, name);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Mail template {name} was not found.", path);
            }

            var template = Template.Parse(File.ReadAllText(path), path);
            if (template.HasErrors)
            {
                throw new Exception($"Mail template {name} failed to parse: {string.Join("; ", template.Messages)}");
            }

            return template;
        });
    }
}
