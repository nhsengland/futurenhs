export interface GenericPageTextContent {
    metaDescription?: string;
    title?: string;
    mainHeading?: string;
    bodyHtml?: string;
    intro?: string;
    navMenuTitle?: string;
    secondaryHeading?: string;
    noResults?:string;
    noResultsMinTermLength?: string;
    subTitle?: string;
}

export interface GroupsPageTextContent extends GenericPageTextContent {
    strapLine?: string;
    firstNameLabel?: string; 
    lastNameLabel?: string;
    pronounsLabel?: string; 
    emailLabel?: string;
}
