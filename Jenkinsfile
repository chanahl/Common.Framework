#!/bin/groovy
/**
 * Jenkinsfile (Declarative Pipeline)
 */

// Map[GitFlow Branch : Build Configuration].
def _configuration = [
  develop : 'Debug',
  feature : 'Debug',
  release : 'Release',
  hotfix : 'Release',
  master : 'Release'
]

// List[.csproj]: Projects expected to produce an asset (.nupkg) on successful build.  Path is relative to workspace.
def _csProjects = [
  "Common.Framework\\Common.Framework.Core\\Common.Framework.Core.csproj",
  "Common.Framework\\Common.Framework.Data\\Common.Framework.Data.csproj",
  "Common.Framework\\Common.Framework.Network\\Common.Framework.Network.csproj",
  "Common.Framework\\Common.Framework.Utilities\\Common.Framework.Utilities.csproj"
]

// String: Git repository name.
def _gitRepositoryName = 'Common.Framework'

// Map[GitFlow Branch : [Jenkins CredentialsId (API Key), Nexus URL ]].
def _nexus = [
  develop : [
    credentialsId : '383c6d87-4ad7-405f-a4c3-3029c76c2818',
    url : 'http://desktop-nns09r8:8081/repository/nuget-private-develop/'],
  feature : [
    credentialsId : '383c6d87-4ad7-405f-a4c3-3029c76c2818',
    url : 'http://desktop-nns09r8:8081/repository/nuget-private-feature/'],
  release : [
    credentialsId : 'de4641c2-8352-40d9-8eae-fa1934f3cafc',
    url : 'http://desktop-nns09r8:8081/repository/nuget-private-release/'],
  hotfix : [
    credentialsId : 'de4641c2-8352-40d9-8eae-fa1934f3cafc',
    url : 'http://desktop-nns09r8:8081/repository/nuget-private-hotfix/'],
  master : [
    credentialsId : 'de4641c2-8352-40d9-8eae-fa1934f3cafc',
    url : 'http://desktop-nns09r8:8081/repository/nuget-private-master/']
]

// String: SonarQube host URL.
def _sonarHostUrl = 'http://desktop-nns09r8:8084'

/**
 * Pipeline
 */
pipeline {
  agent {
    node {
      label 'master'
      customWorkspace "D:\\.ws\\ci\\${_gitRepositoryName}-${BRANCH_NAME}".replaceAll('/', '-')
    }
  }
  
  environment {
    config = null
    gitVersionProperties = null
    nunit = null
    nupkgsDirectory = '.nupkgs'
  }
  
  options {
    buildDiscarder(logRotator(artifactDaysToKeepStr: '', artifactNumToKeepStr: '', daysToKeepStr: '', numToKeepStr: '10'))
    disableConcurrentBuilds()
    timestamps()
  }
  
  tools {
    git "2.12.1.windows.1"
  }
  
  triggers {
    pollSCM('H/5 * * * *')
  }
  
  stages {    
    stage('Clean') {
      steps {
        deleteDir()
      }
    }
    
    stage('Checkout') {
      steps {
        checkout scm
      }
    }
    
    stage('GitVersion') {
      steps {
        bat "${tool name: 'gitversion-4.0.0-beta.12', type: 'com.cloudbees.jenkins.plugins.customtools.CustomTool'} /output buildserver /updateassemblyinfo .\\AssemblyInfo\\Common.Framework.AssemblyInfo.cs"
        script {
          gitVersionProperties = new Properties()
          
          def content = readFile 'gitversion.properties'
          InputStream inputStream = new ByteArrayInputStream(content.getBytes());
          gitVersionProperties.load(inputStream)
          
          currentBuild.displayName = gitVersionProperties.GitVersion_SemVer
        }
      }
    }
    
    stage('NuGet Restore') {
      steps {
        bat "${tool name: 'nuget-4.1.0', type: 'com.cloudbees.jenkins.plugins.customtools.CustomTool'} Restore -ConfigFile %NUGET_CONFIG% -NonInteractive Common.Framework\\Common.Framework.sln"
      }
    }
    
    stage('SonarQube Begin') {
      when {
        expression { BRANCH_NAME ==~ /(develop|master)/ }
      }
      steps {
        script {
          def sonarQubeParameters = sprintf(
            '/k:%1$s /n:%2$s /v:%3$s /d:sonar.host.url=%4$s',
              [
                _gitRepositoryName + "-" + gitVersionProperties.GitVersion_PreReleaseLabel,
                _gitRepositoryName + "-" + gitVersionProperties.GitVersion_BranchName.replaceAll('/', '-'),
                gitVersionProperties.GitVersion_SemVer,
                _sonarHostUrl
              ])
              
          bat "${tool name: 'sonar-scanner-msbuild-3.0.0.629', type: 'hudson.plugins.sonar.MsBuildSQRunnerInstallation'} begin ${sonarQubeParameters}"
        }
      }
    }
    
    stage("Build") {
      steps {
        script {
          def isFutureBranch = BRANCH_NAME.contains('/')
          def branch = isFutureBranch ? BRANCH_NAME.split('/')[0] : BRANCH_NAME
          config = _configuration[branch] ? _configuration[branch] : 'Debug'
          bat "${tool name: 'msbuild-14.0', type: 'msbuild'} Common.Framework\\Common.Framework.sln /p:Configuration=${config} /p:Platform=\"Any CPU\""
        }
      }
      post {
        failure {
          steps {
            script {
              currentBuild.result = 'FAILURE'
            }
          }
        }
      }
    }
    
    stage('SonarQube End') {
      when {
        expression { BRANCH_NAME ==~ /(develop|master)/ }
      }
      steps {
        script {
          bat "${tool name: 'sonar-scanner-msbuild-3.0.0.629', type: 'hudson.plugins.sonar.MsBuildSQRunnerInstallation'} end"
        }
      }
    }
    
    stage('Pack') {
      when {
        environment name: 'currentBuild.result', value: ''
      }
      steps {
        script {
          for (csProject in _csProjects) {
            def packParameters = sprintf(
              '%1$s -Output %2$s -Properties Configuration="%3$s" -Symbols -IncludeReferencedProjects -Version %4$s',
              [
                csProject,
                nupkgsDirectory,
                config,
                gitVersionProperties.GitVersion_SemVer
              ])
            bat "nuget pack ${packParameters}"
          }
        }
      }
    }
    
    stage('Deploy') {
      when {
        environment name: 'currentBuild.result', value: ''
      }
      steps {
        script {
          dir(nupkgsDirectory) {
            def isFutureBranch = BRANCH_NAME.contains('/')
            def branch = isFutureBranch ? BRANCH_NAME.split('/')[0] : BRANCH_NAME
            def credentialsId = _nexus[branch] ? _nexus[branch]['credentialsId'] : ''
            def url = _nexus[branch] ? _nexus[branch]['url'] : ''
            
            withCredentials([
              string(
                credentialsId: credentialsId,
                variable: 'apiKey')]) {
              bat "nuget push *.symbols.nupkg ${apiKey} -Source ${url}"
            }
          }
        }
      }
    }
    
    stage('Tag') {
      when {
        environment name: 'currentBuild.result', value: ''
        expression { BRANCH_NAME ==~ /(develop|master)/ }
      }
      steps {
        script {
          def tagParameters = sprintf(
            '-a %1$s -m "%2$s"',
            [
              gitVersionProperties.GitVersion_SemVer,
              "Tag created by Jenkins."
            ])
          bat "git tag ${tagParameters}"
          
          withCredentials([
            usernamePassword(
              credentialsId: 'd73c882b-5ce2-44e9-a7e1-5549105624eb',
              passwordVariable: 'credentialsPassword',
              usernameVariable: 'credentialsUsername')]) {
            def pushParameters = sprintf(
              'https://%1$s:%2$s@%3$s',
              [
                "${credentialsUsername}",
                "${credentialsPassword}",
                "github.com/chanahl/Common.Framework.git"
              ])
            bat "git push ${pushParameters} --tags"
          }
        }
      }
    }
  }
  
  post {
    always {
      bat 'set > env.out'
    }
    success {
      emailext (
        attachLog: true,
        body: """
          <b>Result:</b> SUCCESS
          <br><br>
          <b>Version:</b> ${gitVersionProperties.GitVersion_SemVer}
          <br><br>
          Check console output at ${BUILD_URL} to view the results.
          <br>""",
        mimeType: 'text/html',
        recipientProviders: [[$class: 'CulpritsRecipientProvider'], [$class: 'DevelopersRecipientProvider']],
        subject: '[JENKINS]: ${PROJECT_NAME}',
        to: 'hlc.alex@gmail.com'
      )
    }
    failure {
      emailext (
        attachLog: true,
        body: """
          <b>Result:</b> FAILURE
          <br><br>
          <b>Version:</b> ${gitVersionProperties.GitVersion_SemVer}
          <br><br>
          Check console output at ${BUILD_URL} to view the results.
          <br>""",
        mimeType: 'text/html',
        recipientProviders: [[$class: 'CulpritsRecipientProvider'], [$class: 'DevelopersRecipientProvider']],
        subject: '[JENKINS]: ${PROJECT_NAME}',
        to: 'hlc.alex@gmail.com'
      )
    }
  }
}
