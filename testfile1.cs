// See https://aka.ms/new-console-template for more information
using System.Net.Http.Headers;
using System.Net;
using Octokit;
using System.Security.Principal;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Linq;
{
    Console.WriteLine("Enter owner");    
    string x = Console.ReadLine();

    Console.WriteLine("Enter repo");
    string y = Console.ReadLine();

    Console.WriteLine("Enter branch");
    string z = Console.ReadLine();

    string accesstoken = "";
    var Client = new GitHubClient(new Octokit.ProductHeaderValue("Octokit-Test"));
    Client.Credentials = new Credentials(accesstoken);


    // github variables
    var owner = x;
    var repo = y;
    var branch = z;

    var targetFile = "https://github.com/" + owner + "/" + repo;

    //try
    //{
    //    //// try to get the file (and with the file the last commit sha)
    //    //var existingFile = await ghClient.Repository.Content.GetAllContentsByRef(owner, repo, targetFile, branch);

    //    //// update the file
    //    //var updateChangeSet = await ghClient.Repository.Content.UpdateFile(owner, repo, targetFile,
    //    //   new UpdateFileRequest("API File update", "Hello Universe! " + DateTime.UtcNow, existingFile.First().Sha, branch));

    //    var repository = Client.Repository.Get(owner, repo);
    //    Branch branch1 = await Client.Repository.Branch.Get(owner, repo, branch);


    //}
    //catch (Octokit.NotFoundException)
    //{
    //    // if file is not found, create it
    //    var createChangeSet = await Client.Repository.Content.CreateFile(owner, repo, targetFile, new CreateFileRequest("API File creation", "Hello Universe! " + DateTime.UtcNow, branch));
    //}

    //Console.WriteLine("Enter new repo");


    //string RepositoryName = Console.ReadLine();
    ////string RepositoryName = "NewTestRepoOctoKit";



    //var contextDelete = Client.Repository.Get(owner, RepositoryName);
    //var reporResult = contextDelete.IsCompletedSuccessfully ? contextDelete.Result : null;
    //long repositoryID = reporResult != null ? reporResult.Id : 0;

    //try
    //{
    //    var repository = new NewRepository(RepositoryName)
    //    {
    //        AutoInit = false,
    //        Description = "",
    //        LicenseTemplate = "mit",
    //        Private = false
    //    };


    //    if (!contextDelete.IsCompletedSuccessfully)
    //    {
    //        //var contextcreate = Client.Repository.Create(repository);

    //        var newRepository = Task.Run(async () => await Client.Repository.Create(repository)).GetAwaiter().GetResult();

    //        repositoryID = newRepository.Id;
            
    //        Console.WriteLine($"The respository {RepositoryName} was created.");
    //    }
    //    else
    //    {
    //        Console.WriteLine($"The respository already {RepositoryName} created.");
    //    }
    //}
    //catch (AggregateException e)
    //{
    //    Console.WriteLine($"E: For some reason, the repository {RepositoryName}  can't be created. It may already exist. {e.Message}");
    //}

    //// Create  Repo
    ////{


    //    //var contextdelete = Client.Repository.Delete(repositoryID);
    //    //OauthToken
    //    //var delRepository = Task.Run(async () => await Client.Repository.Delete(owner, RepositoryName)).GetAwaiter().GetResult();

    //    // Console.WriteLine($"The respository {RepositoryName} was deleted.");

        
    ////}

    //// Create File
    //CreateFileRequest cfr = new CreateFileRequest($"First commit for {""}", "Empty", "main");
    //var result = await Client.Repository.Content.CreateFile(owner, RepositoryName, "testfile1.cs", cfr);

    //var sha = result.Commit.Sha;

    ////await Client.Repository.Content.UpdateFile(owner, RepositoryName, "testfile1.cs", new UpdateFileRequest("My updated file", "New file update", sha));
    //// test



    //// Git Clone
    
    //var trees = Client.Git.Tree.GetRecursive(owner, RepositoryName, sha).Result;

    //Client.Repository.CreateRepository(newRepository, account.Login, account.IsUser)
    //    .Select(repository => cloneService.CloneRepository(repository.CloneUrl, repository.Name, directory))
    //    .SelectUnit();

    // Git Fetch
    // Git Pull
    // Git Push


    var repositoryRead = await Client.Repository.Get(owner, repo);
    var commits = await Client.Repository.Commit.GetAll(repositoryRead.Id);

    List<GitHubCommit> commitList = new List<GitHubCommit>();

    foreach (GitHubCommit commit in commits)
    {
        commitList.Add(commit);
    }

    // Git Clone

    var trees = Client.Git.Tree.GetRecursive(owner, repo, commitList[0].Sha).Result;



    //get the directory contents
    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, String.Format("https://api.github.com/repos/{0}/{1}/contents/", owner, repo));
    request.Headers.Add("Authorization","Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", accesstoken, "x-oauth-basic"))));
    request.Headers.Add("User-Agent", "lk-github-client");

    HttpClient hc = new HttpClient();
    //parse result
    HttpResponseMessage response = await hc.SendAsync(request);
    String jsonStr = await response.Content.ReadAsStringAsync(); ;
    response.Dispose();
    FileInfogit[] dirContents = JsonConvert.DeserializeObject<FileInfogit[]>(jsonStr);

    //read in data
    Directory result;
    result.name = repo;
    result.subDirs = new List<Directory>();
    result.files = new List<FileData>();
    foreach (FileInfogit file in dirContents)
    {
        if (file.type == "dir")
        { //read in the subdirectory
            //Directory sub = await readDirectory(file.name, Client, file._links.self, accesstoken);
            //result.subDirs.Add(sub);
        }
        else
        { //get the file contents;
            HttpRequestMessage downLoadUrl = new HttpRequestMessage(HttpMethod.Get, file.download_url);
            downLoadUrl.Headers.Add("Authorization",
                "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", accesstoken, "x-oauth-basic"))));
            request.Headers.Add("User-Agent", "lk-github-client");

            HttpResponseMessage contentResponse = await hc.SendAsync(downLoadUrl);
            String content = await contentResponse.Content.ReadAsStringAsync();
            contentResponse.Dispose();

            FileData data;
            data.name = file.name;
            data.contents = content;

            result.files.Add(data);
        }
    }
}
struct FileInfo
{
    public String name;
    public String type;
    public String download_url;
    public LinkFields _links;
}

struct LinkFields
{
    public String self;
}
struct FileInfogit
{
    public String name;
    public String type;
    public String download_url;
    public LinkFields _links;
}

//Structs used to hold file data
public struct FileData
{
    public String name;
    public String contents;
}
public struct Directory
{
    public String name;
    public List<Directory> subDirs;
    public List<FileData> files;
}
