pipeline {
    agent any

    stages {
        stage('Prune local only tags') {
            steps {
                bat 'git fetch --prune origin "+refs/tags/*:refs/tags/*"'
            }
        }
        stage('Build') {
            steps {
                powershell './build.ps1'
            }
        }
    }
    post {
        always {
           step([$class: 'CoberturaPublisher', autoUpdateHealth: false, autoUpdateStability: false, coberturaReportFile: 'built/coverage.cobertura.xml', failUnhealthy: false, failUnstable: false, maxNumberOfBuilds: 0, onlyStable: false, sourceEncoding: 'ASCII', zoomCoverageChart: false])
           step([$class: 'MSTestPublisher', testResultsFile: '**/testresults*.xml', failOnError: false, keepLongStdio: true])
        }
        success {
            script {
                def tag = powershell (returnStdout: true, script: '$semVer = (git-flow-version | ConvertFrom-Json).SemVer; git tag $semVer;git push --verbose origin $semVer;$semVer')
                echo "Tag: ${tag}"

                if (tag?.trim()) {
                    slackSend (color: '#008000', message: "Built OK: ${env.JOB_NAME} <${env.BUILD_URL}|#${env.BUILD_NUMBER}> `${tag.trim()}`")
                } else {
                    slackSend (color: '#008000', message: "Built OK: ${env.JOB_NAME} <${env.BUILD_URL}|#${env.BUILD_NUMBER}>")
                }
            }
        }
        // unstable {}
        failure {
            slackSend (color: '#800000', message: """Build Failed: ${env.JOB_NAME} <${env.BUILD_URL}|#${env.BUILD_NUMBER}>
            Commit SHA: ${env.GIT_COMMIT}""")
        }
        // changed {}
    }
}