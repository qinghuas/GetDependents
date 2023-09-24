using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.RegularExpressions;

var page_count = 0;
var dashes = new string('-', 90);
var first_mark = false;
double total_page = 0;

IWebDriver driver = new ChromeDriver();
WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10)); // Wait up to 10 seconds
List<ProjectInfo> projects = new List<ProjectInfo>();
driver.Navigate().GoToUrl("https://github.com/radzenhq/radzen-blazor/network/dependents");

while (true)
{
    ++page_count;
    Thread.Sleep(1500);
    if (!first_mark)
    {
        IWebElement element = driver.FindElement(By.XPath("/html/body/div[1]/div[4]/div/main/turbo-frame/div/div/div/div[2]/div/div[2]/div[1]/div/div[1]/a[1]"));
        // 获取元素的全部文本
        var fullText = element.Text.Replace(",", "");
        // 使用正则表达式提取数字部分
        Regex regex = new Regex(@"\d+");
        Match match = regex.Match(fullText);
        if (match.Success)
        {
            total_page = Math.Ceiling((double)int.Parse(match.Value) / 30);
            first_mark = true;
        }
    }
    // 找到<div class="Box">元素
    IWebElement boxElement = driver.FindElement(By.XPath("/html/body/div[1]/div[4]/div/main/turbo-frame/div/div/div/div[2]/div/div[2]"));
    // 找到所有子元素
    ReadOnlyCollection<IWebElement> childElements = boxElement.FindElements(By.XPath("./*"));
    // 遍历所有子元素
    for (var i = 2; i < childElements.Count; i++)
    {
        try
        {
            var html_content = childElements[i].Text;
            var lines = html_content.Split('\n');
            var firstLineParts = lines[0].Split(new[] { ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);

            ProjectInfo project = new ProjectInfo
            {
                AuthorName = firstLineParts[0],
                ProjectName = firstLineParts[1].Replace("\r", ""),
                StarCount = int.Parse(lines[1].Replace(",", "")),
                ForkCount = int.Parse(lines[2].Replace(",", "")),
                RepoLink = $"https://github.com/{firstLineParts[0]}/{firstLineParts[1].Replace("\r", "")}"
            };

            // 添加到列表中
            projects.Add(project);
            Console.WriteLine(project);
        }
        catch
        {
            continue;
        }
    }

    Console.WriteLine(dashes);
    Console.WriteLine($"current page is {page_count} / {total_page}");
    Console.WriteLine(dashes);

    var attempts = 0;
    var success = false;

    do
    {
        attempts++;
        try
        {
            Thread.Sleep(1500);
            IWebElement element = driver.FindElement(By.LinkText("Next"));
            element.Click();
            success = true;
        }
        catch (Exception)
        {
            Console.WriteLine("Unable to find the element, retrying...");
        }
    } while (!success && attempts < 5);

    if (attempts == 5)
    {
        break;
    }
}

projects = projects.Distinct().ToList();
var sortedProjects = projects.OrderByDescending(p => p.StarCount).ToList();
var json = JsonSerializer.Serialize(sortedProjects);
var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"projects_{timestamp}.json");
File.WriteAllText(filePath, json);

Console.WriteLine("All task done.");
Console.WriteLine(dashes);
public class ProjectInfo
{
    public string? AuthorName
    {
        get; set;
    }
    public string? ProjectName
    {
        get; set;
    }
    public int StarCount
    {
        get; set;
    }
    public int ForkCount
    {
        get; set;
    }

    public string? RepoLink
    {
        get; set;
    }
    public override string ToString()
    {
        return $"AuthorName: {AuthorName}, ProjectName: {ProjectName}, StarCount: {StarCount}, ForkCount: {ForkCount}";
    }
    public override bool Equals(object obj)
    {
        if (obj is not ProjectInfo project)
            return false;

        return AuthorName == project.AuthorName &&
               ProjectName == project.ProjectName &&
               StarCount == project.StarCount &&
               ForkCount == project.ForkCount &&
               RepoLink == project.RepoLink;
    }

    public override int GetHashCode()
    {
        var hash = 17;
        hash = hash * 23 + (AuthorName != null ? AuthorName.GetHashCode() : 0);
        hash = hash * 23 + (ProjectName != null ? ProjectName.GetHashCode() : 0);
        hash = hash * 23 + StarCount.GetHashCode();
        hash = hash * 23 + ForkCount.GetHashCode();
        return hash;
    }
}