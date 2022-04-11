import { useEffect, useRef } from 'react';
import classNames from 'classnames';
import Autocomplete from 'accessible-autocomplete/react';

import { getSiteUsersByTerm } from '@services/getSiteUsersByTerm';
import { RichText } from '@components/RichText';
import { RemainingCharacterCount } from '@components/RemainingCharacterCount';
import { Option } from '@appTypes/option';
import { Service } from '@appTypes/service';

import { Props } from './interfaces';

export const AutoComplete: (props: Props) => JSX.Element = ({
    input: {
        name,
        value,
        onChange
    },
    initialError,
    meta: {
        touched,
        error,
        submitError
    },
    text,
    shouldRenderRemainingCharacterCount,
    validators,
    context,
    services = {
        getSiteUsersByTerm 
    },
    serviceId,
    className
}) => {

    const cache = useRef({});
    const serviceTimeOut = useRef(null);

    const { label, hint } = text ?? {};
    const id: string = name;
    const shouldRenderError: boolean = Boolean(initialError) || ((Boolean(error) || Boolean(submitError)) && touched);
    const isRequired: boolean = Boolean(validators?.find(({ type }) => type === 'required'));
    const maxLength: boolean = validators?.find(({ type }) => type === 'maxLength')?.maxLength;

    const generatedIds: any = {
        remainingCharacters: `${name}-remaining-characters`
    };

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-form-group', className, {
            ['nhsuk-form-group--error']: shouldRenderError
        }),
        label: classNames('nhsuk-label'),
        hint: classNames('nhsuk-hint'),
        error: classNames('nhsuk-error-message'),
        input: classNames('nhsuk-input nhsuk-u-width-full', {
            ['nhsuk-input--error']: shouldRenderError,
            ['u-clearfix']: shouldRenderRemainingCharacterCount && maxLength
        })
    };

    const handleGetSuggestions = async (term: string, callBack: Function): Promise<void> => {

        const service: Service = services[serviceId];
        const cachedData: Array<Option> = cache.current?.[term]; 

        if(!service){

            throw new Error('Invalid service');

        }

        if(cachedData){

            callBack(cachedData);

        } else {

            window.clearTimeout(serviceTimeOut.current);

            serviceTimeOut.current = window.setTimeout(() => {

                services.getSiteUsersByTerm(Object.assign({ ...context, term }))
                    .then(({ data }) => {

                        if(Object.keys(cache.current).length > 10){
        
                            cache.current = {};
                
                        }
            
                        cache.current[term] = data;
                
                        callBack(data);

                    })
                    .catch((error) => {

                        console.log(error);

                    });
    
            }, 500);

        }

    }

    const handleSuggestionConfirm = (selection: Option): any => {

        selection && onChange(selection.value);

    }

    useEffect(() => {

        return () => {

            window.clearTimeout(serviceTimeOut?.current);

        }

    }, []);

    return (

        <div className={generatedClasses.wrapper}>
            <label
                htmlFor={id}
                className={generatedClasses.label}>
                    {label}
            </label>
            {hint &&
                <RichText
                    id={generatedIds.hintId}
                    className={generatedClasses.hint}
                    bodyHtml={hint}
                    wrapperElementType="span" />
            }
            {shouldRenderError &&
                <span className={generatedClasses.error}>{error || submitError || initialError}</span>
            }
            <Autocomplete
                id={name}
                name={name}
                autoselect={true}
                confirmOnBlur={true}
                templates={{
                    inputValue: (value) => value?.label ?? '',
                    suggestion: ({ label }) => label
                }}
                minLength={3}
                displayMenu="overlay"
                required={isRequired}
                showNoOptionsFound={false}
                source={handleGetSuggestions}
                onConfirm={handleSuggestionConfirm} />
                    {(shouldRenderRemainingCharacterCount && maxLength) &&
                        <RemainingCharacterCount
                            id={generatedIds.remainingCharacters}
                            currentCharacterCount={value?.length ?? 0}
                            maxCharacterCount={maxLength}
                            className="u-float-right" />
                    }
        </div>

    )

}
