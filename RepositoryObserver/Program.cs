using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryObserver
{
    class Program
    {
        static void Main(string[] args)
        {
            string repoClone = args[0];
            string dispatcherAddress = "tcp://localhost:8888";
            var repoObserver = new RepoObserver(repoClone, dispatcherAddress);
            repoObserver.ObserveRepoChanges();
        }
    }
}
