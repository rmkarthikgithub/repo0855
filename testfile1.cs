// See https://aka.ms/new-console-template for more information
using System.Net.Http.Headers;
using System.Net;
using Octokit;
using System.Security.Principal;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Reactive;
using static System.Net.WebRequestMethods;
using System.Text;
{
    Console.WriteLine("Enter owner");
    string x = Console.ReadLine();

    Console.WriteLine("Enter repo");
    string y = Console.ReadLine();

    Console.WriteLine("Enter branch");
    string z = Console.ReadLine();

    Console.WriteLine("Enter accesstoken");
    string accesstoken = Console.ReadLine();

    var Client = new GitHubClient(new Octokit.ProductHeaderValue("Octokit-Test"));
    Client.Credentials = new Credentials(accesstoken);


    // github variables
    var owner = x;
    var repo = y;
    var branch = z;

    var targetFile = "https://github.com/" + owner + "/" + repo;

    //// Git Clone

    var repositoryRead = await Client.Repository.Get(owner, repo);
    var commits = await Client.Repository.Commit.GetAll(repositoryRead.Id);

    List<GitHubCommit> commitList = new List<GitHubCommit>();

    foreach (GitHubCommit commit in commits)
    {
        commitList.Add(commit);
    }

    // Git Clone

    var trees = Client.Git.Tree.GetRecursive(owner, repo, commitList[0].Sha).Result;
    List<TreeItem> dirContent = trees.Tree.ToList();

    // working folder

    string subPath = "c:\\\\srcdntu\\" + repo;
    bool exists = System.IO.Directory.Exists(subPath);

    if (!exists)
    {
        System.IO.Directory.CreateDirectory(subPath);
    }
    else
    {
        System.IO.Directory.Delete(subPath, true); System.IO.Directory.CreateDirectory(subPath);
    }

    foreach (TreeItem itm in dirContent)
    {

        if (itm.Type != "blob")
        {
            bool subfldr = System.IO.Directory.Exists(subPath + "\\" + itm.Path);
            if (!subfldr)
            {
                System.IO.Directory.CreateDirectory(subPath + "\\" + itm.Path);
            }
        }
        else
        {

            string filepath = subPath + "\\" + itm.Path;

            HttpRequestMessage requestmsg = new HttpRequestMessage(HttpMethod.Get, String.Format("https://api.github.com/repos/{0}/{1}/contents/", owner, repo));
            requestmsg.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", accesstoken, "x-oauth-basic"))));
            requestmsg.Headers.Add("User-Agent", "lk-github-client");

            HttpRequestMessage downLoadUrl = new HttpRequestMessage(HttpMethod.Get, itm.Url);
            downLoadUrl.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", accesstoken, "x-oauth-basic"))));
            requestmsg.Headers.Add("User-Agent", "lk-github-client");

            HttpClient hclst = new HttpClient();
            HttpResponseMessage contentResponse = await hclst.SendAsync(downLoadUrl);
            String content = await contentResponse.Content.ReadAsStringAsync();
            contentResponse.Dispose();
            // write to file on desktop
            System.IO.File.WriteAllText(filepath
                ,
                content,
                Encoding.UTF8);
        }
    }

    // Git Fetch
    // Git Pull
    // Git Push



}
