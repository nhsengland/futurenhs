# Setup

Setup guide (DACPAC): https://cdsdigital.visualstudio.com/FutureNHS/_wiki/wikis/FutureNHS.wiki/469/DACPAC-Setup-Guide
Repository (Local Automation): https://cdsdigital.visualstudio.com/FutureNHS/_git/FutureNHS-Automation

# The Solution Layout

## FutureNHS.Data.Db
The FutureNHS.Data.Db is the database solution. All tables for groups, forums, posts etc etc

## gulp
The gulp tasks available will either build in debug (an admin user, constants like file status), or automation.

The task to rebuild in each of these modes are:
`gulp buildDb` - builds MvcForum database and Identity database in debug mode
`gulp automationDb` - builds MvcForum database in automation mode (BUG - misaligned config names means when building in automation the .dacpac file for identity db is not found)

Tasks to run individually:
`gulp dropMvcForumDb`
`gulp dropIdentityDb`
`gulp buildMvcForumDatabase`
`gulp buildAutomationMvcForumDatabase`
`gulp buildIdentityDatabase`
`gulp buildAutomationIdentityDatabase`
`gulp deployMvcForumDatabase`
`gulp deployAutomationMvcForumDatabase`
`gulp deployIdentityDatabase`
`gulp deployAutomationIdentityDatabase`

* check validity of above in gulpfile.js

# Work to be done
## Incorporating with MvcForum solution (Check with Tim + Richard A before)

* include sqlproj files in MvcForum solution file
* in the main gulpfile include gulpfile in this directory to make the above commands accessable
* add some of the above commands to the `gulp activate` sequence to build the db with dapper.
* turn off entity framework migrations (remove line from Startup.cs which runs pending migrations on startup - MigrationHistory table is then redundant.)
