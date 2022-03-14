export interface GenericPageTextContent {
    metaDescription?: string;
    title?: string;
    subTitle?: string;
    mainHeading?: string;
    intro?: string;
    navMenuTitle?: string;
    secondaryHeading?: string;
    noResults?:string;
    noResultsMinTermLength?: string;
}

export interface GroupsPageTextContent extends GenericPageTextContent {
    strapLine?: string;
    firstNameLabel?: string; 
    lastNameLabel?: string;
    pronounsLabel?: string; 
    emailLabel?: string;
}
