import type {Screen} from './types';

/** Tracks visited screens so back actions return to the actual previous route. */
export default function createNavigationHistory() {
    const entries:Screen[]=[];
    return {
        push(current:Screen,next:Screen) {
            if (current!==next) entries.push(current);
        },
        back(current:Screen,fallback:Screen) {
            while (entries.length) {
                const previous=entries.pop()!;
                if (previous!==current) return previous;
            }
            return fallback;
        },
        clear() {
            entries.length=0;
        },
    };
}
