import { BreadCrumbList } from '@appTypes/routing';

declare interface Config {
    pathElementList: Array<string>;
}

export const getBreadCrumbList = ({
    pathElementList
}: Config): BreadCrumbList => {

    const breadCrumbElementList: BreadCrumbList = [];

    try {

        if(pathElementList?.length){

            breadCrumbElementList.push({
                element: '',
                text: 'Home'
            });
    
            pathElementList.forEach((item, index) => {
    
                if(index < pathElementList.length -1){
        
                    breadCrumbElementList.push({
                        element: item,
                        text: item.replace('-', ' ')
                    });
        
                }
        
            });
    
        }
    
        return breadCrumbElementList;

    } catch(error){

        return [];

    }

};
