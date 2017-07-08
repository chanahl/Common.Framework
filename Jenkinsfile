#!/bin/groovy
/**
 * Global Variables: https://issues.jenkins-ci.org/browse/JENKINS-41335
 */

// Map[GitFlow Branch : Build Configuration].
def _configuration = [
        develop: 'Debug',
        feature: 'Debug',
        release: 'Release',
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

// String: Custom workspace directory.
def _customWorkspace = 'D:\\.ws\\ci'

// String: Git repository name.
def _gitRepositoryName = 'Common.Framework'

// Map[GitFlow Branch : [Jenkins Credentials ID (API Key), Nexus URL ]].
def _nexus = [
        develop: [
                id : '383c6d87-4ad7-405f-a4c3-3029c76c2818',
                url: 'http://desktop-nns09r8:8081/repository/nuget-private-develop/'
        ],
        feature: [
                id : '383c6d87-4ad7-405f-a4c3-3029c76c2818',
                url: 'http://desktop-nns09r8:8081/repository/nuget-private-feature/'
        ],
        release: [
                id : 'de4641c2-8352-40d9-8eae-fa1934f3cafc',
                url: 'http://desktop-nns09r8:8081/repository/nuget-private-release/'
        ],
        hotfix : [
                id : 'de4641c2-8352-40d9-8eae-fa1934f3cafc',
                url: 'http://desktop-nns09r8:8081/repository/nuget-private-hotfix/'
        ],
        master : [
                id : 'de4641c2-8352-40d9-8eae-fa1934f3cafc',
                url: 'http://desktop-nns09r8:8081/repository/nuget-private-master/'
        ]
]

// String: NuGet.config
def _nugetConfig = 'D:\\.devops\\.nuget\\nuget.config'

// String: SonarQube host URL.
def _sonarHostUrl = 'http://desktop-nns09r8:8084'

/**
 * Jenkinsfile (Declarative Pipeline)
 */
pipeline {
    agent {
        node {
            label 'master'
            customWorkspace "${_customWorkspace}\\${_gitRepositoryName}-${BRANCH_NAME}".replaceAll('/', '-')
        }
    }

    environment {
        branch = null
        commitSHA1 = null
        config = null
        doStageDeploy = null
        doStageNUnit = null
        gitVersionProperties = null
        msbuildParameters = null
        nunitDirectory = '.nunit-result'
        nupkgsDirectory = '.nupkgs'
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
        stage('Initialize') {
            steps {
                deleteDir()
                script {
                    def isFutureBranch = BRANCH_NAME.contains('/')
                    branch = isFutureBranch ? BRANCH_NAME.split('/')[0] : BRANCH_NAME

                    config = _configuration[branch] ? _configuration[branch] : 'Debug'

                    doStageDeploy = _csProjects.empty ? false : true

                    msbuildParameters = sprintf(
                            '%1$s /p:Configuration="%2$s" /p:Platform="%3$s"',
                            [
                                    "Common.Framework\\Common.Framework.sln",
                                    config,
                                    "Any CPU"
                            ])
                }
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
                    InputStream inputStream = new ByteArrayInputStream(content.getBytes())
                    gitVersionProperties.load(inputStream)

                    currentBuild.displayName = gitVersionProperties.GitVersion_SemVer
                }
            }
            post {
                always {
                    bat "\"${tool name: '2.12.1.windows.1', type: 'git'}\" rev-parse HEAD > commitSHA1"
                    script {
                        commitSHA1 = readFile('commitSHA1').trim()
                    }
                }
            }
        }

        stage('NuGet Restore') {
            steps {
                bat "${tool name: 'nuget-4.1.0', type: 'com.cloudbees.jenkins.plugins.customtools.CustomTool'} Restore -ConfigFile ${_nugetConfig} -NonInteractive Common.Framework\\Common.Framework.sln"
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
                bat "${tool name: 'msbuild-14.0', type: 'msbuild'} ${msbuildParameters}"
            }
            post {
                failure {
                    script {
                        currentBuild.result = 'FAILURE'
                        doStageNUnit = false
                    }
                }
                success {
                    script {
                        doStageNUnit = false
                    }
                }
            }
        }

        stage('SonarQube End') {
            when {
                expression { BRANCH_NAME ==~ /(develop|master)/ }
            }
            steps {
                bat "${tool name: 'sonar-scanner-msbuild-3.0.0.629', type: 'hudson.plugins.sonar.MsBuildSQRunnerInstallation'} end"
            }
        }

        stage('Deploy') {
            when {
                environment name: 'currentBuild.result', value: ''
                expression { return doStageDeploy }
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
                        bat "${tool name: 'nuget-4.1.0', type: 'com.cloudbees.jenkins.plugins.customtools.CustomTool'} pack ${packParameters}"
                    }
                }
            }
            post {
                success {
                    script {
                        dir(nupkgsDirectory) {
                            def credentialsId = _nexus[branch] ? _nexus[branch]['id'] : ''
                            def url = _nexus[branch] ? _nexus[branch]['url'] : ''
                            withCredentials([string(credentialsId: credentialsId, variable: 'apiKey')]) {
                                bat "${tool name: 'nuget-4.1.0', type: 'com.cloudbees.jenkins.plugins.customtools.CustomTool'} push *.symbols.nupkg ${apiKey} -Source ${url}"
                            }
                        }
                    }
                }
            }
        }

        stage('NUnit') {
            when {
                expression { return doStageNUnit }
            }
            steps {
                script {
                    def nunitParameters = sprintf(
                            '%1$s --config="%2$s" --result="%3$s"',
                            [
                                    "ProjectEuler\\ProjectEuler.Test\\bin\\${config}\\ProjectEuler.Test.dll",
                                    config,
                                    "${nunitDirectory}\\Common.Framework.Test.xml"
                            ])
                    bat """MD %nunitDirectory%
                        ${tool name: 'nunit3-console-3.6.1', type: 'com.cloudbees.jenkins.plugins.customtools.CustomTool'} ${nunitParameters}
                        EXIT /B 0"""
                }
            }
            post {
                failure {
                    script {
                        currentBuild.result = 'UNSTABLE'
                    }
                }
                success {
                    nunit testResultsPattern: "**/${nunitDirectory}/Common.Framework.Test.xml",
                            debug: false,
                            keepJUnitReports: true,
                            skipJUnitArchiver: false,
                            failIfNoResults: false
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
                            '-a "%1$s" -m "%2$s"',
                            [
                                    gitVersionProperties.GitVersion_SemVer,
                                    "Tag created by Jenkins."
                            ])
                    bat "\"${tool name: '2.12.1.windows.1', type: 'git'}\" tag ${tagParameters}"

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
                        bat "\"${tool name: '2.12.1.windows.1', type: 'git'}\" push ${pushParameters} --tags"
                    }
                }
            }
        }
    }

    post {
        always {
            bat 'set > env.out'
        }
        // changed {
        // }
        // aborted {
        // }
        failure {
            emailext(
                    attachLog: true,
                    body: """<!DOCTYPE html>
                        <html>
                        <head>
                            <meta content='text/html; charset=UTF-8' http-equiv='Content-Type' />
                        </head>
                        <body>
                        <table style=" width: 100%;word-break: break-all;table-layout: fixed;">
                            <tr>
                                <td>
                                    <center><b>Report</b></center>
                                    <br>
                                    <b>Result:</b> FAILURE</div>
                                    <br><br>
                                    <b>Branch:</b> ${branch}
                                    <br>
                                    <b>Commit:</b> ${commitSHA1}
                                    <br>
                                    <b>Version:</b> ${gitVersionProperties.GitVersion_SemVer}
                                    <br><br>
                                </td>
                            </tr>
                        </table>
                        </html>
                        """,
                    mimeType: 'text/html',
                    recipientProviders: [[$class: 'CulpritsRecipientProvider'], [$class: 'DevelopersRecipientProvider']],
                    subject: '[JENKINS]: ${_gitRepositoryName} - ${BRANCH_NAME}',
                    to: 'hlc.alex@gmail.com'
            )
        }
        success {
            emailext(
                    attachLog: true,
                    body: """<!DOCTYPE html>
                        <html>
                        <head>
                            <meta content='text/html; charset=UTF-8' http-equiv='Content-Type' />
                        </head>
                        <body>
                        <table style=" width: 100%;word-break: break-all;table-layout: fixed;">
                            <tr>
                                <td>
                                    <center><b>Report</b></center>
                                    <br>
                                    <b>Result:</b> SUCCESS</div>
                                    <br><br>
                                    <b>Branch:</b> ${branch}
                                    <br>
                                    <b>Commit:</b> ${commitSHA1}
                                    <br>
                                    <b>Version:</b> ${gitVersionProperties.GitVersion_SemVer}
                                    <br><br>
                                </td>
                            </tr>
                        </table>
                        </html>
                        """,
                    mimeType: 'text/html',
                    recipientProviders: [[$class: 'CulpritsRecipientProvider'], [$class: 'DevelopersRecipientProvider']],
                    subject: '[JENKINS]: ${_gitRepositoryName} - ${BRANCH_NAME}',
                    to: 'hlc.alex@gmail.com'
            )
        }
        // unstable {
        // }
    }
}
