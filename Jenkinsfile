pipeline {
  agent any
  
  environment {
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
        git(url: 'git@github.com:chanahl/Common.Framework.git',
            branch: 'develop',
            credentialsId: '92df16ef-1ebd-4d46-bd70-09927dbb5f43')
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
                "${JOB_BASE_NAME}-" + gitVersionProperties.GitVersion_PreReleaseLabel,
                "${JOB_BASE_NAME}",
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
        success {
          script {
            def tagParameters = sprintf(
              '-a %1$s -m "%2$s"',
              [
                gitVersionProperties.GitVersion_SemVer,
                "Tagged created by Jenkins."
              ])
            bat "git tag ${tagParameters}"
            
            withCredentials([[
              $class: 'UsernamePasswordMultiBinding',
              credentialsId: 'd73c882b-5ce2-44e9-a7e1-5549105624eb',
              usernameVariable: 'credentialsUsername',
              passwordVariable: 'credentialsPassword']]) {
                def pushParameters = sprintf(
                  'https://%1$s:%2$s@%3$s',
                  [
                    "${credentialsUsername}",
                    "${credentialsPassword}",
                    "git@github.com:chanahl/Common.Framework.git"
                  ])
                bat "git push ${pushParameters} --tags"
              }
          }
        }
        failure {
          steps {
            script {
              currentBuild.result = 'FAILURE'
              nunit = false
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
  }
  
  post {
    always {
      emailext (
        subject: "${currentBuild.result}: Job '${env.JOB_NAME} [${env.BUILD_NUMBER}]'",
        body: """<p>${currentBuild.result}: Job '${env.JOB_NAME} [${env.BUILD_NUMBER}]':</p>
                <p>Check console output at &QUOT;<a href='${env.BUILD_URL}'>${env.JOB_NAME} [${env.BUILD_NUMBER}]</a>&QUOT;</p>""",
        to: "${DEFAULT_RECIPIENTS}"
      )
    }
  }
}
