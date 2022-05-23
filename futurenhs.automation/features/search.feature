Feature: Search
    Feature covering search functionality of FNHS

    Background:
        Given I have navigated to '/'
        And I have logged in as a 'user' and accept the cookies
        Then the 'My Groups' header is displayed

    @Core
    Scenario Outline: FNHS97 - Search is available validation
        Given I have navigated to '<url>'
        Then the search bar is available
        Examples:
            | url                                                         |
            | groups/                                                     |
            | groups/aa/                                                  |
            | groups/aa/members                                           |
            | groups/aa/members/7db6f2c4-6bf3-4178-967f-ad3a0092a580      |
            | groups/aa/folders                                           |
            | groups/aa/folders/create                                    |
            | groups/aa/folders/f86d22cf-1b0e-4d24-8120-adc500d224fb      |
            | groups/aa/files/54249d0f-3544-4b76-b6fb-ae0001022c09/detail |
            | groups/aa/forum                                             |
            | groups/aa/forum/create                                      |
            | groups/aa/forum/3fbc0d4e-c2a7-46a0-99e9-ad7f010eb4ad        |
            | groups/discover                                             |
            | /terms-and-conditions                                       |
            | /privacy-policy                                             |

    @Core
    Scenario: FNHS98 - Search results display validation
        When I search for 'Automation'
        And there are '9' search results displayed
        And the 'Automation Admin Group' search result card is displayed
            | Group                                                                            |
            | DO NOT USE - This group is reserved solely for use by our automated test scripts |

    @Core
    Scenario: FNHS99 - Search for a Group by Group Title
        When I search for 'Automation Admin'
        And there are '1' search results displayed
        And the 'Automation Admin Group' search result card is displayed
            | Group                                                                            |
            | DO NOT USE - This group is reserved solely for use by our automated test scripts |
        When I select the 'Automation Admin Group' search result card
        Then the 'Automation Admin Group' header is displayed


    Scenario: FNHS100 - Search for a Group That Doesn't Exist
        When I search for 'Automation Group'
        And there are '0' search results displayed
        And the 'Sorry no results found' textual value is displayed


    Scenario: FNHS101 - Search for a Group Where Not a Member
        When I search for 'Automation Created Group'
        And there are '1' search results displayed
        And the 'Automation Created Group' search result card is displayed
            | Group                                  |
            | A group created to test group creation |
        When I select the 'Automation Created Group' search result card
        Then the 'Automation Created Group' header is displayed


    Scenario: FNHS102 - Search for a Discussion by the Title
        When I search for 'General Discussion Validation'
        And there are '1' search results displayed
        And the 'General Discussion Validation' search result card is displayed
            | Discussion on Automation Admin Group group forum |
            | Discussion for general feature validation        |
        When I select the 'General Discussion Validation' search result card
        Then the 'General Discussion Validation' header is displayed


    Scenario: FNHS103 - Search for a Comment by Comment Text
        When I search for 'Comment for Like test'
        And there are '1' search results displayed
        And the 'Comment on discussion: General Discussion Validation' search result card is displayed
            | Discussion on Automation Admin Group group forum |
            | Comment for Like test                            |
        When I select the 'Comment on discussion: General Discussion Validation' search result card
        Then the 'Comment for Like test' comment card is displayed
            | AA         |
            | auto Admin |
            | 0 likes    |
            | Reply      |

    @NotInLocal
    Scenario: FNHS104 - Search for a File by File Name
        When I search for 'docTest'
        And there are '2' search results displayed
        And the 'DocTest' search result card is displayed
            | File on Automation Admin Group group |
            | Test doc                             |
        When I select the 'DocTest' search result card
        Then the 'docTest' header is displayed

    @NotInLocal
    Scenario: FNHS105 - Search for a File by File Description
        When I search for 'Test doc FNHS105'
        And there are '1' search results displayed
        And the 'DocTest' search result card is displayed
            | File on Automation Admin Group group |
            | Test doc FNHS105                     |
        When I select the 'DocTest' search result card
        Then the 'docTest' header is displayed


    Scenario: FNHS106 - Search for a Folder by Folder Name
        When I search for 'Automation Test Folder'
        And there are '1' search results displayed
        And the 'Automation Test Folder' search result card is displayed
            | Folder on Automation Admin Group group |
        When I select the 'Automation Test Folder' search result card
        Then the 'Automation Test Folder' header is displayed
        And the 'Group Files' table is displayed
            | Type | Name | Description | Modified | Actions |


    Scenario: FNHS107 - Search for a File by Folder Description
        When I search for 'Empty folder for testing'
        And there are '2' search results displayed
        And the 'Public Empty Folder' search result card is displayed
            | Folder on Automation Public Group group |
            | Empty Folder for testing                |
        And the 'Empty Folder' search result card is displayed
            | Folder on Automation Admin Group group |
            | Empty Folder for testing               |
        When I select the 'Empty Folder' search result card
        Then the 'Empty Folder' header is displayed



    Scenario: FNHS108 - Search for a Group By the Strapline
        When I search for 'DO NOT USE - This group is reserved solely for use by our automated test scripts'
        Then there are '6' search results displayed
        And the 'Automation Admin Group' search result card is displayed
            | Group                                                                            |
            | DO NOT USE - This group is reserved solely for use by our automated test scripts |
        When I select the 'Automation Admin Group' search result card
        Then the 'Automation Admin Group' header is displayed
        And the 'DO NOT USE - This group is reserved solely for use by our automated test scripts' textual value is displayed


    Scenario: FNHS109 - Search without any serch term
        When I search for ''
        And there are '0' search results displayed
        And the 'Sorry no results found. Try a search term with at least three characters' textual value is displayed


    Scenario Outline: FNHS110 - Search field boundary validation
        When I search for '<searchTerm>'
        And there are '<results>' search results displayed
        And the '<contentValidation>' textual value is displayed
        Examples:
            | searchTerm | results | contentValidation                                                        |
            | at         | 0       | Sorry no results found. Try a search term with at least three characters |
            | aut        | 11      | Automation Admin Group                                                   |