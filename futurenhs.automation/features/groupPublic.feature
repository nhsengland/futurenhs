Feature: groupPublic
    User journeys covering functionality and permissions when working with a public group

    Background:
        Given I have navigated to '/'
        Given I have logged in as a 'user' and accept the cookies
        Then the 'My Groups' header is displayed
        And I have navigated to '/groups/automation-public-group'
        Then the 'Automation Public Group' header is displayed


    Scenario: FNHS43 - View Forum of Public Group
        When I click the 'Forum' tab
        Then the 'All Discussions' header is displayed
        And the 'New Discussion' link is not displayed
        And the 'Public Discussion Test' discussion card is displayed
            | AA                                 |
            | Created by auto Admin [PRETTYDATE] |
            | Last comment by                    |
            | Comments: 11                       |
        When I select the 'Public Discussion Test' discussion card
        Then the 'A discussion to validate public access/view in read only format' textual value is displayed
        And the 'Comments: 11' textual value is displayed
        And the 'First Public Comment' comment card is displayed
            | AA                |
            | auto Admin        |
            | 0 likes           |
            | Reply             |
            | Show more replies |
        And the 'First Public Reply' reply card is displayed
            | AA         |
            | auto Admin |
            | 0 likes    |
        And the 'Add Comment' button is not displayed


    Scenario: FNHS44 - View Files and Folders of Public Group
        When I click the 'Files' tab
        Then the 'Files' header is displayed
        And the 'Add Folder' link is not displayed
        When I click the 'Public Empty Folder' link
        Then the 'Public Empty Folder' header is displayed
        And the 'Upload a file' link is not displayed
        And the 'Add Folder' link is not displayed
        And the 'Edit Folder' link is not displayed
        And the 'Delete Folder' link is not displayed
        And the 'Group Files' table is displayed
            | Type | Name    | Description | Modified                                      | Actions                     |
            |      | docTest | test doc    | 10 Jan 2022\nBy auto Admin\nAuthor auto Admin | Download file\nView details |
        When I click 'View details' on the 'docTest' row of the 'Group Files' table
        Then the 'docTest' header is displayed
        And the 'test doc' textual value is displayed
        And the 'File Details' table is displayed
            | Name    | Modified by | Last update | Actions  |
            | docTest | auto Admin  | 10 Jan 2022 | Download |
        And the 'Download' link is displayed

    @Core
    Scenario: FNHS45 - Join a public group
        And the 'Join group' button is displayed
        When I click the 'Join group' button
        Then the 'Join group' button is not displayed
        When I click the 'Members' tab
        Then the 'auto User' row is displayed on the 'Group Members' table


    Scenario: FNHS46 - Cancel leaving a group
        When I select 'Leave group' from the group actions accordion
        Then I cancel this on the open dialog
        When I open the 'Group actions' accordion
        And the 'Leave group' button is displayed

    @Core
    Scenario: FNHS47 - Leave a group
        When I select 'Leave group' from the group actions accordion
        Then I confirm this on the open dialog
        And the 'Join group' button is displayed