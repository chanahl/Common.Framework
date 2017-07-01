pipeline {
  agent any
  stages {
    stage('Clean') {
      steps {
        deleteDir()
      }
    }
    stage('SCM') {
      steps {
        git(url: 'git@github.com:chanahl/Common.Framework.git', branch: 'develop', credentialsId: '92df16ef-1ebd-4d46-bd70-09927dbb5f43')
      }
    }
    stage('NuGet Restore') {
      steps {
        bat '%NUGET_RESTORE_COMMAND% Common.Framework.sln'
      }
    }
    stage('GitVersion') {
      steps {
        bat '%GITVERSION_EXE% /output buildserver /updateassemblyinfo .\\AssemblyInfo\\Common.Framework.AssemblyInfo.cs'
      }
    }
  }
  environment {
    gitVersionProperties = null
    nunit = null
  }
}
