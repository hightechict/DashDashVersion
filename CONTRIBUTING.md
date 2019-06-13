# Getting Started

If you want to contribute to DashDashVersion you are more than welcome.  
Contributions can take many forms like submitting issues, writing documentation or making code changes.

* Make sure you have a [GitHub account](https://github.com/signup/free)
* Submit a ticket for your issue, assuming one does not already exist.
  * Clearly describe the issue including the steps to reproduce when it is a bug.
  * Make sure you fill in the earliest version that you know has the issue.
* Fork the repository on GitHub, then clone it using your Git client.
* Make sure the project builds and all tests pass on your machine.
* Make sure to use [git flow](https://atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow)

## Making Changes

* Create a feature branched off develop.
* Implement your feature or bug fix. Please following existing coding styles and do not introduce new ones.
* Make atomic, focused commits with [good commit messages](https://chris.beams.io/posts/git-commit/).
* Make sure you have added the necessary tests for your changes.
* Run all tests to assure nothing else was accidentally broken.
* Add an entry to the [change log](CHANGELOG.md) using the guidelines in [keep a changelog](https://keepachangelog.com/en/1.0.0/).

## Submitting Changes

* Push your changes to a topic branch in your fork of the repository.
* Send a Pull Request targeting the develop branch. Note what issue/issues your patch fixes.

Some things that will increase the chance that your pull request is accepted.

* Following existing code conventions.
* Using [dotnet format](https://github.com/dotnet/format).
* Including unit tests that would otherwise fail without the patch, but pass after applying it.
* Updating the documentation and tests that are affected by the contribution.
* If code from elsewhere is used, proper credit and a link to the source should exist in the code comments.  
  Then licensing issues can be checked against our license.

## Additional Resources

* [General GitHub documentation](http://help.github.com/)
* [GitHub pull request documentation](https://help.github.com/articles/using-pull-requests/)
