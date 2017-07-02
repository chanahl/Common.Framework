pipeline {
  agent node {
    customWorkspace "D:\\.ws\\${ITEM_FULL_NAME}"
  }
  
  environment {
    gitUrl = 'git@github.com:chanahl/Common.Framework.git'
    gitBranch = 'develop'
    gitCredentialsId = '92df16ef-1ebd-4d46-bd70-09927dbb5f43'
    gitVersionProperties = null
    nunit = null
  }
  
  options {
    buildDiscarder(logRotator(artifactDaysToKeepStr: '', artifactNumToKeepStr: '', daysToKeepStr: '', numToKeepStr: '10'))
    disableConcurrentBuilds()
    timestamps()
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
    
    stage('SCM') {
      steps {
        git(url: gitUrl, branch: gitBranch, credentialsId: gitCredentialsId)
      }
    }
    
    stage('NuGet Restore') {
      steps {
        bat '%NUGET_RESTORE_COMMAND% Common.Framework\\Common.Framework.sln'
      }
    }
    
    stage('GitVersion') {
      steps {
        bat '%GITVERSION_EXE% /output buildserver /updateassemblyinfo .\\AssemblyInfo\\Common.Framework.AssemblyInfo.cs'
        
        script {
          gitVersionProperties = new Properties()
          
          def content = readFile 'gitversion.properties'
          InputStream inputStream = new ByteArrayInputStream(content.getBytes());
          gitVersionProperties.load(inputStream)
          
          currentBuild.displayName = gitVersionProperties.GitVersion_SemVer
        }
      }
    }
    
    stage('SonarQube Begin') {
      steps {
        script {
          def sonarQube = tool name: 'sonar-scanner-msbuild-3.0.0.629', type: 'hudson.plugins.sonar.MsBuildSQRunnerInstallation'
          def sonarQubeParameters = sprintf(
            '/k:%1$s /n:%2$s /v:%3$s /d:sonar.host.url=%4$s',
              [
                "Common.Framework-" + gitVersionProperties.GitVersion_PreReleaseLabel,
                "Common.Framework-${JOB_BASE_NAME}",
                gitVersionProperties.GitVersion_SemVer,
                "http://desktop-nns09r8:8084"
              ])
              
          bat "${sonarQube}\\SonarQube.Scanner.MSBuild.exe begin ${sonarQubeParameters}"
        }
      }
    }
    
    stage("Build") {
      steps {
        bat "${tool name: 'msbuild-14.0', type: 'msbuild'} Common.Framework\\Common.Framework.sln /p:Configuration=\"Debug\" /p:Platform=\"Any CPU\""
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
      steps {
        script {
          def sonarQube = tool name: 'sonar-scanner-msbuild-3.0.0.629', type: 'hudson.plugins.sonar.MsBuildSQRunnerInstallation'
          bat "${sonarQube}\\SonarQube.Scanner.MSBuild.exe end"
        }
      }
    }
    
    stage('Tag') {
      when {
        environment name: 'currentBuild.result', value: ''
      }
      steps {
        script {
          def tagParameters = sprintf(
            '-a %1$s -m "%2$s"',
            [
              gitVersionProperties.GitVersion_SemVer,
              "Tagged created by Jenkins."
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
    success {
      emailext (
        attachLog: true,
        body: '''
          <b>Result:</b> SUCCESS
          <br><br>
          Check console output at ${BUILD_URL} to view the results.
          <br>''',
        mimeType: 'text/html',
        recipientProviders: [[$class: 'CulpritsRecipientProvider'], [$class: 'DevelopersRecipientProvider']],
        subject: '[JENKINS]: ${PROJECT_NAME}',
        to: 'hlc.alex@gmail.com'
      )
    }
    failure {
      emailext (
        attachLog: true,
        body: '''
          <b>Result:</b> FAILURE
          <br><br>
          Check console output at ${BUILD_URL} to view the results.
          <br>''',
        mimeType: 'text/html',
        recipientProviders: [[$class: 'CulpritsRecipientProvider'], [$class: 'DevelopersRecipientProvider']],
        subject: '[JENKINS]: ${PROJECT_NAME}',
        to: 'hlc.alex@gmail.com'
      )
    }
  }
}
