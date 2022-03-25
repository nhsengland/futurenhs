const { IncomingWebhook } = require('@slack/webhook');

class SlackReporter {

    constructor(webhookURL){
        this.webhook = new IncomingWebhook(webhookURL);
    }

    async sendMessage(body){
        await this.webhook.send(body);
    }

    async sendPostMessage(results, baseUrl){
        const postBody = {
            'text': `Ran tests on \`${baseUrl}\``,
            'attachments': [
                {
                    'text': `*Number of Features: ${results.finished}*\n*Passed: ${results.passed}*\n*Failed: ${results.failed}*`,
                }
            ]
        };

        await this.sendMessage(postBody);
    }
}
module.exports = SlackReporter;