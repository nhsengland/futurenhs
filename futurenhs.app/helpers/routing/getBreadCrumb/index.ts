import { BreadCrumbList } from '@appTypes/routing';

declare interface Config {
    pathElementList: Array<string>;
    shouldIncludeAllParams?: boolean;
}

export const getBreadCrumbList = ({
    pathElementList,
    shouldIncludeAllParams
}: Config): BreadCrumbList => {

    const breadCrumbElementList: BreadCrumbList = [];
    const lastIndex: number = shouldIncludeAllParams ? pathElementList?.length : pathElementList?.length -1

    try {

        if(pathElementList?.length){

            // breadCrumbElementList.push({
            //     element: '',
            //     text: 'Home'
            // });
    
            pathElementList.forEach((item, index) => {
    
                if(index < lastIndex){
        
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
