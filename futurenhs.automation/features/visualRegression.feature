Feature: Visual Regression
    As a user of the website
    I want to navigate the site
    To do cross comparison image analysis


Scenario: FNHS:V01 - High Level Page Regression
    Given I have navigated to '/'
    Then the 'Log In' header is displayed
    And the 'loginPage' page image is taken and compared to the baseline image
    And I have logged in as an 'visreguser' and accept the cookies
    Then the 'My Groups' header is displayed
    And the 'myGroups' page image is taken and compared to the baseline image
    When I select the 'Automation Visual Regression Group' group card
    Then the 'Automation Visual Regression Group' header is displayed
    And the 'groupHome' page image is taken and compared to the baseline image
    When I click the 'Forum' tab
    Then the 'All Discussions' header is displayed
    And the 'groupForumTab' page image is taken and compared to the baseline image
    When I click the 'Files' tab
    Then the 'Files' header is displayed
    And the 'groupFilesTab' page image is taken and compared to the baseline image
    When I click the 'Members' tab
    Then the 'Group Members' table exists
    And the 'groupMembersTab' page image is taken and compared to the baseline image
    # When I click the 'About Us' tab
    # Then the 'About Us' header is displayed
    # And the 'groupAboutUsTab' page image is taken and compared to the baseline image


Scenario Outline: FNHS:V02 - Public Pages Regression
    Given I have navigated to '/'
    And I have navigated to '<URL>'
    Then the '<page>' page image is taken and compared to the baseline image
    And I have navigated to '/members/register'
    Then the 'register' page image is taken and compared to the baseline image
Examples:
    | URL                   | <page>             |
    | /terms-and-conditions | termsAndConditions |
    | /privacy-policy       | privacyPolicy      |
    | /cookies              | cookies            | 
    | /contact-us           | contactUs          |



Scenario: FNHS:V03 - Group Forum validation
    Given I have navigated to '/'
    Then the 'Log In' header is displayed
    And I have logged in as an 'visreguser'and accept the cookies
    Then the 'My Groups' header is displayed
    When I select the 'Automation Visual Regression Group' group card
    Then the 'Automation Visual Regression Group' header is displayed
    When I click the 'Forum' tab
    Then the 'All Discussions' header is displayed
    When I click the 'New Topic 1' link
    Then the 'New Topic 1' header is displayed
    And the 'groupDiscussion' page image is taken and compared to the baseline image


Scenario: FNHS:V04 - Group Files validation
    Given I have navigated to '/'
    Then the 'Log In' header is displayed
    And I have logged in as an 'visreguser'and accept the cookies
    Then the 'My Groups' header is displayed
    When I select the 'Automation Visual Regression Group' group card
    Then the 'Automation Visual Regression Group' header is displayed
    When I click the 'Files' tab
    Then the 'Files' header is displayed
    When I click the 'Base Folder 1' link
    Then the 'Base Folder 1' header is displayed
    And the 'groupFolders' page image is taken and compared to the baseline image
    When I click the 'test pdf' link
    Then the 'test pdf' header is displayed
    And the 'groupFilePreview' page image is taken and compared to the baseline image


Scenario: FNHS:V05 - Group Member Profile validation
    Given I have navigated to '/'
    Then the 'Log In' header is displayed
    And I have logged in as an 'visreguser'and accept the cookies
    Then the 'My Groups' header is displayed
    When I select the 'Automation Visual Regression Group' group card
    Then the 'Automation Visual Regression Group' header is displayed
    When I click the 'Members' tab
    Then the 'Group Members' table exists
    When I click the 'Vis Reg' link
    Then the 'Member Profile' header is displayed
    And the 'groupMemberProfile' page image is taken and compared to the baseline image

@Pending
Scenario: FNHS:V06 - Group Actions form pages validation
    Given I have navigated to '/'
    Then the 'Log In' header is displayed
    And I have logged in as an 'visreguser'and accept the cookies
    Then the 'My Groups' header is displayed
    When I select the 'Automation Visual Regression Group' group card
    Then the 'Automation Visual Regression Group' header is displayed
    When I select 'Add new member' from the group pages accordion
    Then the 'Invite member' header is displayed
    And the 'groupAddNewMember' page image is taken and compared to the baseline image
    When I select 'Invite new user' from the group pages accordion
    Then the 'Invite new member to join this group' header is displayed
    And the 'groupInviteNewUser' page image is taken and compared to the baseline image
    When I select 'Edit group information' from the group pages accordion
    Then the 'Edit group' header is displayed
    And the 'groupEditInformation' page image is taken and compared to the baseline image


Scenario: FNHS:V07 - Mobile View validation
    Given I have navigated to '/'
    And I ensure the browser is in mobile emulation
    Then the 'Log In' header is displayed
    And the 'mobile-loginPage' page image is taken and compared to the baseline image
    Given I have logged in as a 'visreguser'
    When I open the 'Mobile Menu' accordion
    Then the 'Vis Reg' textual value is displayed
    And the 'mobile-menu' page image is taken and compared to the baseline image
    When I click the 'Groups' link
    Then the 'My Groups' header is displayed
    And the 'mobile-myGroups' page image is taken and compared to the baseline image
    When I select the 'Automation Visual Regression Group' group card
    Then the 'Automation Visual Regression Group' header is displayed
    And the 'mobile-groupHome' page image is taken and compared to the baseline image
    When I select 'Members' from the group menu accordion
    Then the 'Members' textual value is displayed
    And the 'mobile-groupMembers' page image is taken and compared to the baseline image