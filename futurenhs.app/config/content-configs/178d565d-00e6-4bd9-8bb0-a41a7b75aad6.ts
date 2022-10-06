import { GenericPageTextContent } from '@appTypes/content'

/**
 * App does find and replace on all variables by %variable% key
 * eg:
 *
 * secondaryHeading.replace('%GROUPNAME%', groupName)
 */
export default {
    title: "You've been invited to join FutureNHS",
    secondaryHeading: '%INVITEDBY% has invited you to join %GROUPNAME%!',
    mainHeading: 'Welcome to FutureNHS',
    bodyHtml: `<p>The FutureNHS platform helps you connect, share, learn and make a difference to the community. 
    Choose to collaborate as a group member or setup and customise your own group.</p>

    <p>Click the button below to sign up.</p>`,
} as GenericPageTextContent
