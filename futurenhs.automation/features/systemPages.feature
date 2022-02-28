@Pending
#Admin portal doesn't exist in vNext this feature is redundant
Feature: siteAdmin
    Tests covering functionality available to a user with site administrator privelages

Background:
    Given I have navigated to '/'
    And I have logged in as a 'admin'


Scenario: FNHS83 - Create system content page
    When I select 'Admin' from the menu accordion
    Then the 'MvcForum' link is displayed
    When I click the 'System Pages' link
    Then the 'System Pages' header is displayed
    And the 'System Pages' table exists
    When I click the 'Create new page' link
    Then the 'Create new page' header is displayed
    When I enter 'Automation Content Page' into the 'Title' field
    And I enter 'automation-content-page' into the 'Slug' field
    And I enter 'There is no strife, no prejudice, no national conflict in outer space as yet. Its hazards are hostile to us all. Its conquest deserves the best of all mankind, and its opportunity for peaceful cooperation many never come again. But why, some say, the moon? Why choose this as our goal? And they may well ask why climb the highest mountain? Why, 35 years ago, fly the Atlantic? Why does Rice play Texas? We choose to go to the moon. We choose to go to the moon in this decade and do the other things, not because they are easy, but because they are hard, because that goal will serve to organize and measure the best of our energies and skills, because that challenge is one that we are willing to accept, one we are unwilling to postpone, and one which we intend to win, and the others, too. It is for these reasons that I regard the decision last year to shift our efforts in space from low to high gear as among the most important decisions that will be made during my incumbency in the office of the Presidency. In the last 24 hours we have seen facilities now being created for the greatest and most complex exploration in man's history. We have felt the ground shake and the air shattered by the testing of a Saturn C-1 booster rocket, many times as powerful as the Atlas which launched John Glenn, generating power equivalent to 10,000 automobiles with their accelerators on the floor. We have seen the site where the F-1 rocket engines, each one as powerful as all eight engines of the Saturn combined, will be clustered together to make the advanced Saturn missile, assembled in a new building to be built at Cape Canaveral as tall as a 48 story structure, as wide as a city block, and as long as two lengths of this field.' into the text editor
    Then the 'Automation Content Page' row is displayed on the 'System Pages' table


Scenario Outline: FNHS84 - Create system page error validation
    When I select 'Admin' from the menu accordion
    Then the 'MvcForum' link is displayed
    When I click the 'System Pages' link
    Then the 'System Pages' header is displayed
    And the 'System Pages' table exists
    When I click the 'Create new page' link
    Then the 'Create new page' header is displayed
    When I enter '<Title>' into the 'Title' field
    And I enter '<Slug>' into the 'Slug' field
    And I enter '<Content>' into the text editor
    Then the '<error message>' textual value is displayed
Examples:
    | Title                                                                                                 | Slug                            | Content                                                                                                 | error message                                                                        |
    |                                                                                                       | slugtest                        | This test is to ensure we cannot create a page without a title being provided                           | The Title field is required                                                          |
    | FmKafa44YCy4SRsa1IodUZhErGeThYHwdlnQqxDhNnMtxsTED9I5uRJKWnFMaECNMBi9cwr0V4Wj5wTv00qyjZXUvT2Im8t6XVnfp | large-title                     | This test is to ensure we cannot create a page with a title of more than 100 characters                 | Title must be less than 100 characters                                               |
    | No slug                                                                                               |                                 | This test is to ensure we cannot create a page with a slug being provided                               | The Slug field is required                                                           |
    | Large Slug                                                                                            | porruf-jwpqhe8nmsfnh-orc67dyoni | This test is to ensure we cannot create a page with a slug of more than 30 characters                   | Slug must be less than 30 characters                                                 |
    | Same Slug                                                                                             | repeat-slug-test                | This test is to ensure we cannot create a page with a slug that already exists on another content page  | The slug provided already exists, it must be unique                                  |
    | Illegal Slug                                                                                          | illegal slug!                   | This test is to ensure that we cannot create a page with a slug using blank space or illegal characters | The slug provided is not valid, it must not contain any special characters or spaces |
    | No Content                                                                                            | no-content                      |                                                                                                         | The Content field is required                                                        |


Scenario: FNHS85 - Validate System Page
    Given I have navigated to '/pages/automation-content-page'
    Then the 'Automation Content Page' header is displayed
    And the 'There is no strife, no prejudice, no national conflict in outer space as yet. Its hazards are hostile to us all. Its conquest deserves the best of all mankind, and its opportunity for peaceful cooperation many never come again. But why, some say, the moon? Why choose this as our goal? And they may well ask why climb the highest mountain? Why, 35 years ago, fly the Atlantic? Why does Rice play Texas? We choose to go to the moon. We choose to go to the moon in this decade and do the other things, not because they are easy, but because they are hard, because that goal will serve to organize and measure the best of our energies and skills, because that challenge is one that we are willing to accept, one we are unwilling to postpone, and one which we intend to win, and the others, too. It is for these reasons that I regard the decision last year to shift our efforts in space from low to high gear as among the most important decisions that will be made during my incumbency in the office of the Presidency. In the last 24 hours we have seen facilities now being created for the greatest and most complex exploration in man's history. We have felt the ground shake and the air shattered by the testing of a Saturn C-1 booster rocket, many times as powerful as the Atlas which launched John Glenn, generating power equivalent to 10,000 automobiles with their accelerators on the floor. We have seen the site where the F-1 rocket engines, each one as powerful as all eight engines of the Saturn combined, will be clustered together to make the advanced Saturn missile, assembled in a new building to be built at Cape Canaveral as tall as a 48 story structure, as wide as a city block, and as long as two lengths of this field.' textual value is displayed 


Scenario: FNHS86 - Edit a System Page  
    Given I create 'Editable System Page' system page
    When I select 'Admin' from the menu accordion
    Then the 'MvcForum' link is displayed
    When I click the 'System Pages' link
    Then the 'System Pages' header is displayed
    When I click 'Edit' on the 'Editable System Page' row of the 'System Pages' table
    Then the 'Edit system page' header is displayed
    And the 'Title' field contains 'Editable System Page'
    When I enter 'Edited Automation Page' into the 'Title' field
    And I click the 'Save' option
    Then the 'Edited Automation Page' row is displayed on the 'System Pages' table


Scenario: FNHS87 - Delete a System Page
    Given I create 'Deletable System Page' system page
    When I select 'Admin' from the menu accordion
    Then the 'MvcForum' link is displayed
    When I click the 'System Pages' link
    Then the 'System Pages' header is displayed
    When I click 'Edit' on the 'Deletable System Page' row of the 'System Pages' table
    Then the 'Edit system page' header is displayed
    And the 'Title' field contains 'Deletable System Page'
    When I confirm and delete the 'Deletable System Page'
    Then the 'System Pages' header is displayed