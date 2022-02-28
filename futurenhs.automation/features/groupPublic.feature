Feature: groupPublic
    User journeys covering functionality and permissions when working with a public group

Background: 
    Given I have navigated to '/'
    Given I have logged in as a 'user'
    When I click the 'Groups' nav icon
    Then the 'My Groups' header is displayed


Scenario: FNHS43 - View Forum of Public Group
    When I click the 'Discover new groups' tab
    When I click the 'Automation Public Group' link
    Then the 'Automation Public Group' header is displayed
    When I click the 'Forum' tab
    Then the 'All Discussions' header is displayed
    And the 'New Discussion' link is not displayed


Scenario: FNHS44 - View Files and Folders of Public Group
    When I click the 'Discover new groups' tab
    When I click the 'Automation Public Group' link
    Then the 'Automation Public Group' header is displayed
    When I click the 'Files' tab
    Then the 'Files' header is displayed
    And the 'Add Folder' link is not displayed
    When I click the 'Public Empty Folder' link   
    Then the 'Public Empty Folder' header is displayed
    And the 'Upload a file' link is not displayed
    And the 'Add Folder' link is not displayed
    And the 'Edit Folder' link is not displayed
    And the 'Delete Folder' link is not displayed
    When I click 'docTest' on the 'docTest' row of the 'Group Files' table
    Then the 'docTest' header is displayed  
    And the 'Download' link is displayed

@Core 
Scenario: FNHS45 - Join a public group
    When I click the 'Discover new groups' tab
    When I click the 'Automation Public Group' link
    Then the 'Automation Public Group' header is displayed
    And the 'Join Group' link is displayed
    When I click the 'Join Group' link
    When I click the 'Members' tab
    Then the 'Auto User' row is displayed on the 'Group Members' table


Scenario: FNHS46 - Cancel leaving a group
    When I click the 'Automation Public Group' link
    Then the 'Automation Public Group' header is displayed
    When I select 'Leave group' from the group pages accordion
    Then I cancel this on the open 'Leave group' dialog
    Then the 'Automation Public Group' header is displayed
    And the 'Leave Group' link is displayed

@Core 
Scenario: FNHS47 - Leave a group 
    When I click the 'Automation Public Group' link
    Then the 'Automation Public Group' header is displayed   
    When I select 'Leave group' from the group pages accordion
    Then I confirm this on the open 'Leave group' dialog
    And the 'Join Group' link is displayed