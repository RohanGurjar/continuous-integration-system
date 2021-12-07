using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepositoryObserver
{
    internal class RepoObserver
    {
        private string _latestCommitSha;

        public string RepoClone { get; }
        public string DispatcherAddress { get; }

        public RepoObserver(string repoClone, string dispatcherAddress)
        {
            RepoClone = repoClone;
            DispatcherAddress = dispatcherAddress;
        }

        internal void ObserveRepoChanges()
        {
            while (true)
            {
                Thread.Sleep(1000);
                var basePath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
                if (File.Exists($@"{basePath}/.commit"))
                {
                    _latestCommitSha = File.ReadAllText($@"{basePath}/.commit");
                    File.Delete($@"{basePath}/.commit");
                }

                using (var repo = new Repository($@"{basePath}/test_repo_clone_obs"))
                {
                    UpdateRepo(repo);

                    string latestCommitSha = repo.Commits.First().Sha;
                    if (_latestCommitSha != latestCommitSha)
                    {
                        File.WriteAllText($@"{basePath}/.commit", latestCommitSha);
                    }
                }
            }
        }

        /// <summary>
        ///     https://github.com/libgit2/libgit2sharp/wiki/git-pull
        /// </summary>
        /// <param name="repo"></param>
        private void UpdateRepo(Repository repo)
        {
            var localMain = repo.Branches["main"];
            Commands.Checkout(repo, localMain);
            repo.Reset(ResetMode.Hard, repo.Head.Tip);

            var options = new PullOptions();
            var signature = new Signature(new Identity("rohan", "a@b.com"), DateTimeOffset.Now);
            Commands.Pull(repo, signature, options);
        }
    }
}
