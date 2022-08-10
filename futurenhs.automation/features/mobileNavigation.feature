Feature: mobileNavigation
    Tests covering unique mobile manu navigation functionality available to a user access the site via mobile web app

    Background: Log In
        Given I have navigated to '/'
        And I ensure the browser is in mobile emulation
        Then the 'Log In' header is displayed
        And the 'Register your interest' header is displayed
        And I have logged in as an 'user' and accept the cookies

    
    Scenario: FNHS:M01 - Menu Validation
        When I open the 'Mobile Menu' accordion
        Then the 'auto User' textual value is displayed
        And the 'Groups' link is displayed
        And the 'Need help?' textual value is displayed
        And the 'Visit our support site' link is displayed
        When I open the 'user menu' accordion
        Then the 'My profile' link is displayed
        And the 'Log Off' link is displayed

    
    Scenario: FNHS:M02 - All Groups Validation
        Then the 'My Groups' header is displayed
        And the 'Collaborate without boundaries' header is displayed
        And the 'Automation Admin Group' group card is displayed
            | DO NOT USE - This group is reserved solely for use by our automated test scripts |
        When I select 'Discover new groups' from the group pages accordion
        Then the 'Discover new Groups' header is displayed
        And the 'Supercharge your knowledge' header is displayed

    
    Scenario: FNHS:M03 - Group Members Table Validation
        Then the 'My Groups' header is displayed
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed
        When I select 'Members' from the group menu accordion
        Then the 'Group Members' mobile table is displayed
            | Auto GroupAdmin |                  |
            | Role            | Admin            |
            | Date joined     | [PrettyDate]     |
            | Last logged in  | [PrettyDate]     |
            | auto User       |                  |
            | Role            | Standard Members |
            | Date joined     | [PrettyDate]     |
            | Last logged in  | [PrettyDate]     |
    
    
    Scenario: FNHS:M04 - Search for a group
        When I open the 'Mobile Menu' accordion
        When I search for 'Automation Admin'
        And there are '1' search results displayed
        And the 'Automation Admin Group' search result card is displayed
            | Group                                                                            |
            | DO NOT USE - This group is reserved solely for use by our automated test scripts |
        When I select the 'Automation Admin Group' search result card
        Then the 'Automation Admin Group' header is displayed

    
    Scenario: FNHS:M05 - Join a public group
        And I have navigated to '/groups/automation-public-group'
        Then the 'Automation Public Group' header is displayed
        And the 'Join group' button is displayed
        When I click the 'Join group' button
        Then the 'Join group' button is not displayed
        When I select 'Members' from the group menu accordion
        Then the 'auto User' row is displayed on the 'Group Members' table

    
    Scenario: FNHS:M06 - View group member profile page
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed
        When I select 'Members' from the group menu accordion
        Then the 'Group members' table exists
        When I click 'auto User' on the 'auto User' row of the 'Group Members' table
        Then the 'Member Profile' header is displayed
        And the 'AU' textual value is displayed
        And the profile values are displayed
            | First name | auto                |
            | Last name  | User                |
            | Email      | autoUser@test.co.uk |


    Scenario: FNHS:M07 - View a forum discussion
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed 
        When I select 'Forum' from the group menu accordion
        Then the 'All Discussions' header is displayed
        Then the 'General Discussion Validation' discussion card is displayed
            | AA                                     |
            | Created by Auto Admin [PRETTYDATE]     |
            | Last comment by Auto User [PRETTYDATE] |
        And the 'General Discussion Validation' discussion card is pinned
        When I select the 'General Discussion Validation' discussion card
        Then the 'General Discussion Validation' header is displayed 
        And the 'First Comment' comment card is displayed
            | AA                |
            | auto Admin        |
            | 0 likes           |
            | Reply             |
            | Show more replies |
        And the 'First blank reply' reply card is displayed
            | AU                                        |
            | auto User                                 |
            | In response to auto Admin "First Comment" |
            | 0 likes                                   |
            | Reply                                     |
        When I open the 'Show more replies' accordion

        
    Scenario: FNHS:M08 - Files and Folders page validation
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed 
        When I select 'Files' from the group menu accordion
        Then the 'Files' header is displayed
        And the 'Add Folder' link is displayed
        And the 'Group Files' mobile table is displayed
            | Folder                 |
            | 4000DescTest           |
            | Description            |
            |                        |
            | Folder                 |
            | AutoFolder             |
            | Description            |
            |                        |
            | Folder                 |
            | Automation Test Folder |
        When I click the 'Automation Test Folder' link
        Then the 'Automation Test Folder' header is displayed
        And the 'Add Folder' link is displayed
        And the 'Upload File' link is displayed
        And the 'Group Files' mobile table is displayed
            | Folder |
            | Folder 1 |
            | |
            | |
            | |
            | .doc |
            | docTest |
            | Description |
            | |
            | Download file |
            | .pdf |
            | pdfTest |
            | Description |
            | |
            | Download file |
            | .xlsx |
            | test excel |
            | Description |
            | |
            | Download file |
            | .pptx |
            | test ppt |
            | Description |
            | |
            | Download file |


    Scenario: FNHS:M09 - Leave a group
        And I have navigated to '/groups/aa'
        Then the 'Automation Admin Group' header is displayed
        When I select 'Leave group' from the group actions accordion
        Then I confirm this on the open dialog
        And the 'Join group' button is displayed