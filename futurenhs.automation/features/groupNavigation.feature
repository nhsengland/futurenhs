Feature: Groups
    User Journeys around general navigation for Groups on the Future NHS platform

Background:
    Given I have navigated to '/'
    And I have logged in as a 'user' and accept the cookies
    Then the 'My Groups' header is displayed

@Core
Scenario: FNHS09 - My groups validation
    Then the 'All my groups' header is displayed
    And the 'Share ideas, get advice and support from your peers' textual value is displayed
    And the 'Automation Admin Group' group card is displayed
    | DO NOT USE - This group is reserved solely for use by our automated test scripts |
    | Members: 2Discussions: 7                                                         |

@Core 
Scenario: FNHS10 - Discover new groups validation
    When I click the 'Discover new groups' tab
    Then the 'Discover New Groups' header is displayed
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
    When I open the 'Actions' accordion
    Then the 'Leave group' link is displayed
    When I click the 'Forum' tab
    Then the 'All Discussions' header is displayed
    When I click the 'Files' tab
    Then the 'Files' header is displayed
    When I click the 'Members' tab
    Then the 'Group Members' table exists
    # When I click the 'About Us' tab
    # Then the 'About Us' header is displayed
    When I click the 'Home' tab
    And the 'DO NOT USE - This group is reserved solely for use by our automated test scripts' textual value is displayed

# WILL NEED MORE WORK WHEN PAGE BUILDER IS DEVELOPED
Scenario: FNHS15 -  Group home validation
    When I select the 'Automation Admin Group' group card
    Then the 'Automation Admin Group' header is displayed
    And the 'DO NOT USE - This group is reserved solely for use by our automated test scripts' textual value is displayed
    # And the PAGE BUILDER ADMIN TEST IS NOT DISPLAYED


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
    # | Views: 0                                |


Scenario: FNHS17 - Group Members validation
    When I select the 'Automation Admin Group' group card
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Members' tab
    And the 'Group Members' table exists

@Pending
Scenario: FNHS18 - Group about us validation
    When I select the 'Automation Admin Group' group card
    Then the 'Automation Admin Group' header is displayed
    When I click the 'About Us' tab
    Then the 'About Us' header is displayed
    And the 'DO NOT USE - This group is reserved solely for use by our automated test scripts' textual value is displayed
    And the 'Group Rules' header is displayed
    And the 'Baatezu subtype bonus creation subschool divination druid fear aura granted power lawful magical beast type mentalism domain nauseated sorcerer spell domain total concealment travel domain. Abjuration alignment alternate form blindsight construct type damage fate domain fly free action halfling domain improved evasion infection lawful melee miniature figure modifier natural reach plant type player character result spell descriptor spell immunity strength domain subschool suffering domain swarm subtype unarmed attack untrained.' textual value is displayed


Scenario: FNHS21 - Group Members, Table Validation
    When I select the 'Automation Admin Group' group card
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Members' tab
    And the 'Group Members' table is displayed
    | Name            | Role             | Date joined  | Last logged in |
    | Auto GroupAdmin | Admin            | [PrettyDate] | [PrettyDate]   |
    # | auto Test  | Standard Members | [PrettyDate] | [PrettyDate]   |
    | auto User       | Standard Members | [PrettyDate] | [PrettyDate]   |

@Pending
## PENDING MEMBERS NOT A FEATURE IN PRIVATEBETA
Scenario: FNHS22 - Group Members, Pending Table Does Not Exist
    When I select the 'Automation Admin Group' group card
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Members' tab
    And the 'Pending members' table is not displayed