Feature: Groups
    User Journeys around general navigation for Groups on the Future NHS platform

    Background:
        Given I have navigated to '/'
        And I have logged in as a 'user' and accept the cookies
        Then the 'My Groups' header is displayed

    @Core
    Scenario: FNHS09 - My groups validation
        Then the 'Collaborate without boundaries' header is displayed
        And the 'Connect, share and learn.' textual value is displayed
        And the 'Automation Admin Group' group card is displayed
            | DO NOT USE - This group is reserved solely for use by our automated test scripts |
            | Members: 3Discussions: 7                                                         |

    @Core
    Scenario: FNHS10 - Discover new groups validation
        When I click the 'Discover new groups' tab
        Then the 'Discover new Groups' header is displayed
        And the 'Automation Private Group' group card is displayed
            | DO NOT USE - This group is reserved solely for use by our automated test scripts |
            | Members: 2Discussions: 0                                                         |
        And the 'Automation Public Group' group card is displayed
            | DO NOT USE - This group is reserved solely for use by our automated test scripts |
            | Members: 1Discussions: 1                                                         |
        And the 'Automation Visual Regression Group' group card is displayed
            | DO NOT USE - This group is reserved solely for use by our automated test scripts |
            | Members: 2Discussions: 3                                                         |


    @Core
    Scenario: FNHS14 - Group tabs navigation
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed
        When I open the 'Group actions' accordion
        Then the 'Leave group' button is displayed
        When I click the 'Forum' tab
        Then the 'All Discussions' header is displayed
        When I click the 'Files' tab
        Then the 'Files' header is displayed
        When I click the 'Members' tab
        Then the 'Group Members' table exists
        When I click the 'Home' tab
        Then the 'Welcome to FutureNHS' header is displayed


    Scenario: FNHS15 -  Group user home validation
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed
        When I open the 'Group actions' accordion
        Then the 'Edit group information' link is not displayed
        And the 'Leave group' button is displayed
        And the 'Welcome to FutureNHS' header is displayed
        And the 'A platform from the NHS, helping the health and social care sector connect and collaborate' textual value is displayed
        And the 'Key Links' header is displayed
        And the 'FutureNHS Support' link is displayed
        And the 'NHS England' link is displayed
        And the 'NHS Website' link is displayed


    Scenario: FNHS16 - Group forum validation
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed
        When I click the 'Forum' tab
        Then the 'All Discussions' header is displayed
        And the 'New Discussion' link is displayed
        Then the 'General Discussion Validation' discussion card is displayed
            | AA                                      |
            | Created by Auto Admin [PRETTYDATE]      |
            | Last comment by Auto Admin [PRETTYDATE] |
            | Comments: 15                            |
    # | Views: 0                                | VIEWS DEPRECATED FEATURE AWAITING REQUIREMENTS


    Scenario: FNHS17 - Group Members validation
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed
        When I click the 'Members' tab
        And the 'Group Members' table exists

    
    Scenario: FNHS18 - Group about us validation
        Given I have navigated to '/groups/automation-private-group'
        Then the 'Automation Private Group' header is displayed
        When I click the 'About us' tab
        Then the 'About Us' header is displayed
        And the 'Join group' button is displayed    
    

    Scenario: FNHS21 - Group Members, Table Validation
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed
        When I click the 'Members' tab
        And the 'Group Members' table is displayed
            | Name            | Role             | Date joined  | Last logged in |
            | Auto GroupAdmin | Admin            | [PrettyDate] | [PrettyDate]   |
            | auto RemoveUser | Standard Members | [PrettyDate] | [PrettyDate]   |
            # | auto Test  | Standard Members | [PrettyDate] | [PrettyDate]   |
            | auto User       | Standard Members | [PrettyDate] | [PrettyDate]   |


    Scenario: FNHS22 - Group Members, Pending Table Does Not Exist
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed
        When I click the 'Members' tab
        And the 'Pending members' table is not displayed