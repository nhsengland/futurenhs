Feature: Groups
    User Journeys around general navigation for Groups on the Future NHS platform

Background:
    Given I have navigated to '/'
    And I have logged in as a 'user'
    When I click the 'Groups' nav icon
    Then the 'My Groups' header is displayed


Scenario: FNHS09 - My groups validation
    Then the 'All my groups' header is displayed
    And the 'Share ideas, get advice and support from your peers' textual value is displayed
    And the 'Automation Admin Group' link is displayed

@Core 
Scenario: FNHS10 - Discover new groups validation
    When I click the 'Discover new groups' tab
    Then the 'Discover New Groups' header is displayed
    And the 'Automation Private Group' link is displayed
    And the 'Automation Public Group' link is displayed
    And the 'Automation Visual Regression Group' link is displayed


Scenario: FNHS11 - Group Card Validation
    And the 'Automation Admin Group' group card is displayed
    | DO NOT USE - This group is reserved solely for use by our automated test scripts |
    | 3 Members7 discussions                                                           |


Scenario: FNHS13 - Group Card (No Discussion) Validation
    When I click the 'Discover new groups' tab
    Then the 'Discover New Groups' header is displayed
    Then the 'Automation Public Group' group card is displayed
    | DO NOT USE - This group is reserved solely for use by our automated test scripts |
    | 1 Member0 Discussions                                                            |

@Core
Scenario: FNHS14 - Group tabs navigation
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    When I open the 'Actions' accordion
    Then the 'Leave group' link is displayed
    When I click the 'Forum' tab
    Then the 'All Discussions' header is displayed
    When I click the 'Files' tab
    Then the 'Files' header is displayed
    When I click the 'Members' tab
    Then the 'Group Members' table exists
    When I click the 'About Us' tab
    Then the 'About Us' header is displayed
    When I click the 'Home' tab
    And the 'DO NOT USE - This group is reserved solely for use by our automated test scripts' textual value is displayed


Scenario: FNHS15 -  Group home validation
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    And the 'DO NOT USE - This group is reserved solely for use by our automated test scripts' textual value is displayed
    And the 'DO NOT USE - This group is reserved solely for use by our automated test scripts' header is displayed


Scenario: FNHS16 - Group forum validation
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Forum' tab
    Then the 'All Discussions' header is displayed
    And the 'New Discussion' link is displayed


Scenario: FNHS17 - Group Members validation
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Members' tab
    And the 'Group Members' table exists


Scenario: FNHS18 - Group about Us page validation
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    When I click the 'About Us' tab
    Then the 'About Us' header is displayed
    And the 'DO NOT USE - This group is reserved solely for use by our automated test scripts' textual value is displayed
    And the 'Group Rules' header is displayed
    And the 'Baatezu subtype bonus creation subschool divination druid fear aura granted power lawful magical beast type mentalism domain nauseated sorcerer spell domain total concealment travel domain. Abjuration alignment alternate form blindsight construct type damage fate domain fly free action halfling domain improved evasion infection lawful melee miniature figure modifier natural reach plant type player character result spell descriptor spell immunity strength domain subschool suffering domain swarm subtype unarmed attack untrained.' textual value is displayed


Scenario: FNHS21 - Group Members, Table Validation
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Members' tab
    And the 'Group Members' table is displayed
    | Name       | Role             | Date joined  | Last logged in | 
    | auto Admin | Admin            | [PrettyDate] | [PrettyDate]   | 
    | auto Test  | Standard Members | [PrettyDate] | [PrettyDate]   | 
    | auto User  | Standard Members | [PrettyDate] | [PrettyDate]   | 


Scenario: FNHS22 - Group Members, Pending Table Does Not Exist
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Members' tab
    And the 'Pending members' table is not displayed