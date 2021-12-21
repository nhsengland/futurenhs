import { format, formatDistance, subDays } from 'date-fns'

export const dateTime = ({ relativeDayThreshold = 3 }): Function => {

    return (value: any): string => {

        try {

            if (value) {

                if(relativeDayThreshold && subDays(new Date(), relativeDayThreshold) < new Date(value)){

                    return formatDistance(new Date(value), new Date(), { 
                        addSuffix: true 
                    });

                }

                return format(new Date(value), 'dd MMM yyyy');

            }

            return value;

        } catch(error) {

            return 'An unexpected error occured';

        }

    };

};
