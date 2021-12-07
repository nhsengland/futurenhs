export interface GenericPageContent {
    metaDescriptionText: string;
    titleText: string;
    mainHeadingHtml: string;
}

export interface GroupsPageContent extends GenericPageContent {
    introHtml: string;
    navMenuTitleText: string;
    secondaryHeadingHtml: string;
}
